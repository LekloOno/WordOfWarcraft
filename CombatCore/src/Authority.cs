using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using CombatCore.Primitives;
using CombatCore.Sources;
using CombatCore.Sources.Attachment;
using CombatCore.Sources.Lifecycle;
using CombatCore.Status;

namespace CombatCore;

/// <summary>
/// Resolves requests through fixed stages, one at a time, FIFO (anything emitted while resolving is queued):
///   fuse -> replace -> add -> modify -> resolve -> publish fact (reactions enqueue derived requests).
/// The stages are shared by every request type; only 'resolve' is type specific.
/// </summary>
public sealed class Authority
{
    readonly IGameClock _clock;
    readonly IEntityDirectory _entities;

    public Authority(IGameClock clock, IEntityDirectory entities)
    {
        _clock = clock;
        _entities = entities;
        // The lifecycle processor is DRIVEN by this authority and private to it: every lifecycle command, whoever submits it
        // (game code, a layer above, a hook, a ticket), becomes a SourceRequest of this pipeline. It is broadcast, can be intercepted,
        // and waits in the same FIFO as every other request, so it can never overtake a request emitted before it.
        _sources = new SourcesProcessor(new SourceSink(this), RouteSourceCommand);
    }

    sealed class Handle(Action onDispose) : IDisposable
    {
        Action? _a = onDispose;
        public void Dispose() { _a?.Invoke(); _a = null; }
    }

    sealed record Entry<T>(T Item, long Timestamp);

    interface IPipeline { }

    sealed class Pipeline<T> : IPipeline where T : Request
    {
        public readonly List<Entry<Replacement<T>>> Replacements = [];
        public readonly List<Entry<Addition<T>>> Additions = [];
        public readonly List<Entry<Modifier<T>>> Modifiers = [];
    }

    /// <summary>
    /// Type-safe heterogeneous container: C# cannot say "Type T maps to Pipeline&lt;T&gt;", so the one unavoidable cast
    /// lives here, and the rest of the authority only ever sees Pipeline&lt;T&gt;.
    /// </summary>
    sealed class PipelineSet
    {
        readonly Dictionary<Type, IPipeline> _map = new();
        public Pipeline<T> Get<T>() where T : Request =>
            (Pipeline<T>)(_map.TryGetValue(typeof(T), out var p) ? p : _map[typeof(T)] = new Pipeline<T>());
    }

    interface IFactChannel { }
    sealed class FactChannel<T> : IFactChannel { public readonly List<Func<T, IEnumerable<Draft>>> Handlers = []; }

    public int MaxDepth { get; init; } = 8;
    public List<string> Log { get; } = [];
    public long Now { get; private set; }
    readonly Dictionary<EntityId, StatusRegistry> _statuses = [];
    readonly Queue<Request> _queue = new();
    readonly SourcesProcessor _sources;
    FactCtx? _applyCtx;                                // set while a SourceRequest is being applied: lifecycle facts carry it
    readonly Dictionary<EntityId, Source> _entityNodes = [];
    uint _nextSource;                                  // 0 is the System root
    readonly PipelineSet _pipelines = new();
    readonly Dictionary<Type, IFactChannel> _facts = [];
    uint _nextRequest, _nextStatus;
    long _nextTimestamp;
    bool _running;

    // ---------------- queries ----------------
    internal StatusRegistry StatusRegistryOf(EntityId e)
    {
        if (!_statuses.TryGetValue(e, out var registry)) _statuses[e] = registry = new();
        return registry;
    }

    public IReadOnlyCollection<StatusInstance> StatusesOf(EntityId e) =>
        _statuses.TryGetValue(e, out var l) ? l.Instances : Array.Empty<StatusInstance>();
    public bool HasTag(EntityId e, GrantTag t) => StatusesOf(e).Any(s => s.Def.Grants.Contains(t));

    // ---------------- registration: every call returns a handle ----------------
    Pipeline<T> P<T>() where T : Request => _pipelines.Get<T>();

    public IDisposable Add<T>(Replacement<T> r) where T : Request => Reg(P<T>().Replacements, r);
    public IDisposable Add<T>(Addition<T> a) where T : Request => Reg(P<T>().Additions, a);
    public IDisposable Add<T>(Modifier<T> m) where T : Request => Reg(P<T>().Modifiers, m);

    Handle Reg<T>(List<Entry<T>> list, T item)
    {
        var e = new Entry<T>(item, _nextTimestamp++);
        list.Add(e);
        return new Handle(() => list.Remove(e));
    }

    /// <summary>React to (or observe) a fact. Returned drafts become Derived requests, cause preserved.</summary>
    public IDisposable OnFact<T>(Func<T, IEnumerable<Draft>> handler)
    {
        var list = Channel<T>().Handlers;
        list.Add(handler);
        return new Handle(() => list.Remove(handler));
    }

    FactChannel<T> Channel<T>() =>
        (FactChannel<T>)(_facts.TryGetValue(typeof(T), out var c) ? c : _facts[typeof(T)] = new FactChannel<T>());

    // ---------------- entry points ----------------
    public void Submit(Draft draft)
    {
        Enqueue(draft, Origin.Original, [], 0);
        if (!_running) Run();
    }

    /// <summary>Call once per tick. Reads the clock and expires due statuses (through the pipeline, as Remove requests).</summary>
    public void Update()
    {
        Now = _clock.Tick;
        var due = _statuses.Values.SelectMany(l => l.Instances)
            .Where(s => s.ExpiresAt is { } t && t <= Now)
            .OrderBy(s => s.ExpiresAt).ThenBy(s => s.Id).ToList();
        foreach (var s in due)
            Submit(new StatusDraft(Root, s.Owner, StatusOp.Remove,
                                   RemoveFilter: new StatusFilter(DefId: s.Def.Id), Reason: RemoveReason.Expired));
    }

    /// <summary>Reads the typed data of a status. The typed definition is the key, so two definitions sharing a data type never collide.</summary>
    public bool TryGetData<T>(EntityId entity, TypedStatusDefinition<T> key, [NotNullWhen(true)] out T? data) where T : class
    {
        data = _statuses.TryGetValue(entity, out var registry) ? registry.Find(key.Id)?.Data as T : null;
        return data is not null;
    }

    // ---------------- sources ----------------

    /// <summary>The System node, root of the source tree.</summary>
    public Source Root => _sources.Root;

    /// <summary>A node that is not in the tree yet (Terminated). Ids come from here, nothing to pick by hand.</summary>
    public Source NewSource(string name, SourceTagSet tags = default, ISourceAttachment? attachment = null) =>
        new(++_nextSource, name, tags, attachment);

    // ---- lifecycle requests. `by` = who asks (default: the node acting). Applied when their turn comes in the FIFO. ----

    /// <summary>Asks to put <paramref name="child"/> under <paramref name="parent"/> (born pending if the parent is pending).</summary>
    public void Insert(Source parent, Source child, object? ctx = null, Source? by = null, Source? cause = null) =>
        Submit(new SourceDraft(by ?? parent, SourceCommandKind.Insert, parent, child, 0, ctx, cause));

    /// <summary>Asks for a graceful termination: the node drains (children, then its attachment), then leaves the tree.</summary>
    public void Terminate(Source node, object? ctx = null, Source? by = null, Source? cause = null) =>
        Submit(new SourceDraft(by ?? node, SourceCommandKind.Terminate, node, null, 0, ctx, cause));

    /// <summary>Asks for a hard kill: the whole subtree leaves the tree at once, no waiting.</summary>
    public void Kill(Source node, object? ctx = null, Source? by = null, Source? cause = null) =>
        Submit(new SourceDraft(by ?? node, SourceCommandKind.Kill, node, null, 0, ctx, cause));

    /// <summary>Asks to cancel a pending termination (only the node that started the wave can be aborted).</summary>
    public void Abort(Source node, object? ctx = null, Source? by = null, Source? cause = null) =>
        Submit(new SourceDraft(by ?? node, SourceCommandKind.Abort, node, null, 0, ctx, cause));

    /// <summary>NewSource + Insert (emitted by the parent). Outside a run the node is Active when this returns; during a run, when its turn comes.</summary>
    public Source Spawn(Source parent, string name, SourceTagSet tags = default, ISourceAttachment? attachment = null)
    {
        var node = NewSource(name, tags, attachment);
        Insert(parent, node);
        return node;
    }

    /// <summary>The node of an entity, created on first use under the System root. Gone (and forgotten) once terminated.</summary>
    public Source EntitySource(EntityId entity)
    {
        if (_entityNodes.TryGetValue(entity, out var node)) return node;
        node = NewSource($"Entity {entity}", SourceTagSet.Of(SourceTags.Entity), new EntityAttachment(entity));
        _entityNodes[entity] = node;
        Insert(Root, node);
        return node;
    }

    // What a hook or a ticket submits to the processor lands here: it becomes a request like any other. Submitted while a lifecycle
    // request is being applied, it is DERIVED from it (lineage and depth continue, so the fuse covers lifecycle cascades).
    void RouteSourceCommand(SourceCommand c)
    {
        var draft = new SourceDraft(c.Node, c.Kind, c.Node, c.Other, c.Generation, c.Ctx);
        if (_applyCtx is { } ctx) Enqueue(draft with { Cause = ctx.Cause }, Origin.Derived, ctx.Lineage, ctx.Depth + 1);
        else Enqueue(draft, Origin.Original, [], 0);
        if (!_running) Run();
    }

    /// <summary>
    /// Publishes a fact that has no request of its own, of ANY type: the lifecycle facts, and the facts of the layers built on the
    /// authority (a spell layer announcing "the run stopped", a passive layer...). Called from a lifecycle hook, the fact carries the ctx
    /// of the request being applied (lineage, depth); <paramref name="node"/> becomes its cause when the request has none.
    /// </summary>
    public void Announce<T>(T fact, Source? node = null)
    {
        var ctx = _applyCtx is { } c
            ? c with { Cause = c.Cause ?? node }
            : new FactCtx(default, node, ImmutableArray<long>.Empty, 0);
        Publish(fact, ctx);
        if (!_running) Run();   // outside a run (a game callback): the reactions are resolved now
    }

    // Forgets the lookup entries of a node that left the tree, so a later EntitySource builds a fresh one.
    void Forget(Source node)
    {
        if (node.Attachment is EntityAttachment e && _entityNodes.GetValueOrDefault(e.Id) == node) _entityNodes.Remove(e.Id);
    }

    sealed class SourceSink(Authority authority) : ISourceLifecycleSink
    {
        public void OnInserted(Source parent, Source child) { authority.Log.Add($"SRC   {child} inserted under {parent}"); authority.Announce(new SourceInserted(parent, child), child); }
        public void OnTerminationStarted(Source node) { authority.Log.Add($"SRC   {node} terminating"); authority.Announce(new SourceTerminating(node), node); }
        public void OnTerminated(Source node, bool killed) { authority.Log.Add($"SRC   {node} {(killed ? "killed" : "terminated")}"); authority.Forget(node); authority.Announce(new SourceTerminated(node, killed), node); }
        public void OnAborted(Source node) { authority.Log.Add($"SRC   {node} abort"); authority.Announce(new SourceAborted(node), node); }
        public void OnRejected(SourceCommandKind kind, Source node, SourceRejectReason reason) { authority.Log.Add($"SRC   {kind} on {node} rejected: {reason}"); authority.Announce(new SourceCommandRejected(kind, node, reason), node); }
    }

    // ---------------- entity lifecycle ----------------

    /// <summary>
    /// Call when an entity is about to be freed: ends every status it owns (their effects are disposed).
    /// Requests still queued for it are dropped when they reach resolution (the directory no longer finds it).
    /// </summary>
    public void RemoveEntity(EntityId entity)
    {
        var ctx = new FactCtx(default, null, ImmutableArray<long>.Empty, 0);
        foreach (var inst in StatusesOf(entity).ToList()) RemoveInstance(inst, RemoveReason.OwnerRemoved, ctx);
        _statuses.Remove(entity);

        // Graceful: whatever runs under it is told from outside, but a consequence still in flight (a projectile...) keeps the node, and so its chain, alive.
        if (_entityNodes.TryGetValue(entity, out var node)) Terminate(node, by: Root);
    }

    void Enqueue(Draft d, Origin origin, ImmutableArray<long> lineage, int depth)
    {
        var id = new RequestId(++_nextRequest);
        _queue.Enqueue(d switch
        {
            ResourceDraft r => new ResourceRequest(id, r.Source, r.Cause, origin, lineage, depth, r.Target,
                                                  r.Resource, r.Direction, r.Tags, new BucketedValue(r.Amount)),
            StatusDraft s => new StatusRequest(id, s.Source, s.Cause, origin, lineage, depth, s.Target,
                                               s.Op, s.Def, new BucketedValue(s.Level), new BucketedValue((long)(s.Duration ?? 0)),
                                               s.Duration.HasValue, s.LevelOp, s.TimeOp, s.RemoveFilter, s.Reason),
            SourceDraft c => new SourceRequest(id, c.Source, c.Cause, origin, lineage, depth, c.Node.OwnerOf() ?? default,
                                               c.Command, c.Node, c.Other, c.Generation, c.Ctx),
            _ => throw new NotSupportedException(d.GetType().Name),
        });
    }

    void Run()
    {
        _running = true;
        try
        {
            while (_queue.TryDequeue(out var item))
                switch (item)
                {
                    case ResourceRequest rr: Process(rr, ResolveResource); break;
                    case StatusRequest sr: Process(sr, ResolveStatus); break;
                    case SourceRequest src: Process(src, ResolveSource); break;
                }
        }
        finally { _running = false; }
    }

    static FactCtx CtxOf(Request r) => new(r.Id, r.Cause, r.Lineage, r.Depth);

    // ---------------- the shared stages ----------------
    void Process<T>(T original, Action<T> resolve) where T : Request
    {
        // 1. fuse
        if (original.Depth > MaxDepth)
        {
            Log.Add($"DROP  {original.Id} depth {original.Depth} > {MaxDepth}");
            Publish(new RequestDropped(CtxOf(original), "depth"), CtxOf(original));
            return;
        }

        // (lifecycle commands are exempt: asking twice, or asking about a node that is already gone, is handled by the processor)
        // A node that left the tree has lost its chain (no parent, no inherited context): its requests would match nothing
        // reliably. Pending nodes are fine: that is exactly what "consequences outlive the flow" means.
        if (original is not SourceRequest && original.Source.State is SourceState.Terminated)
        {
            Drop(original, "source terminated", CtxOf(original));
            return;
        }

        Log.Add($"REQ   {original.Describe()}");
        var p = P<T>();

        // 2. replace: one pass ordered by (phase, registration time); each sees the previous output;
        //    an interceptor never acts twice in one lineage.
        var current = (T)original.CloneValues();
        foreach (var e in p.Replacements.OrderBy(x => x.Item.Phase).ThenBy(x => x.Timestamp).ToList())
        {
            var id = e.Timestamp;
            var rep = e.Item;
            if (current.Lineage.Contains(id) || !rep.Match(current)) continue;
            current = (T)rep.Apply(current).WithLineage(current.Lineage.Add(id));
            Log.Add($"REPL  {id}");
        }

        // 3. add: decided on the ORIGINAL request, queued, never resolved inline.
        foreach (var e in p.Additions.OrderBy(x => x.Timestamp).ToList())
        {
            var add = e.Item;
            var id = e.Timestamp;
            if (original.Lineage.Contains(id) || !add.Match(original)) continue;
            foreach (var d in add.Add(original))
            {
                Log.Add($"ADD   {id}");
                Enqueue(d with { Cause = d.Cause ?? original.Cause }, Origin.Derived, original.Lineage.Add(id), original.Depth + 1);
            }
        }

        // 4. modify: on the final shape of the request, derived or not (unless the filter says otherwise).
        foreach (var e in p.Modifiers.OrderBy(x => x.Timestamp).ToList())
        {
            var mod = e.Item;
            var id = e.Timestamp;
            if (!mod.Match(current)) continue;
            int before = current.ContributionCount;
            mod.Apply(current, id);
            if (current.ContributionCount > before) Log.Add($"MOD   {id}");
        }

        // 5/6. resolve (type specific) publishes facts; reactions enqueue derived requests.
        resolve(current);
    }

    void Publish<T>(T fact, FactCtx ctx)
    {
        if (!_facts.TryGetValue(typeof(T), out var ch)) return;
        foreach (var h in ((FactChannel<T>)ch).Handlers.ToList())
            foreach (var d in h(fact))
            {
                Log.Add($"REACT {typeof(T).Name} -> {d.GetType().Name} on {d.Target}");
                Enqueue(d with { Cause = d.Cause ?? ctx.Cause }, Origin.Derived, ctx.Lineage, ctx.Depth + 1);
            }
    }

    // ---------------- resolve: lifecycle ----------------
    void ResolveSource(SourceRequest r)
    {
        // Facts published while it is applied (inserted, terminating, terminated, hooks) carry this request's ctx.
        _applyCtx = CtxOf(r);
        try { _sources.Apply(new SourceCommand(r.Command, r.Node, r.Other, r.Generation, r.Ctx)); }
        finally { _applyCtx = null; }
    }

    // ---------------- resolve: resources ----------------
    void ResolveResource(ResourceRequest current)
    {
        var ctx = CtxOf(current);
        var entity = _entities.Find(current.Target);
        if (entity is null) { Drop(current, "target gone", ctx); return; }
        if (!entity.Resources.Has(current.Resource)) { Log.Add($"NOOP  {current.Id} {current.Target} has no resource {current.Resource.Value}"); return; }

        long amount = current.Value.Resolve();
        var change = entity.Resources.Apply(current.Resource, current.Direction, amount);

        var fact = new ResourceFact(ctx, current.Source, current.Target, current.Resource, current.Direction, current.Tags,
                                    amount, change.Applied, change.Before, change.After, current.Value.Contributions.ToArray());
        Log.Add($"FACT  {current.Id} {current.Direction} res{current.Resource.Value} {current.Target}: {change.Before} -> {change.After}"
                + (fact.Contributions.Count > 0 ? $"  [base {current.Value.Base}, " + string.Join(", ", fact.Contributions.Select(c => $"{c.By}:{c.Kind}{c.Amount:+0.##;-0.##}")) + "]" : ""));
        Publish(fact, ctx);

        if (change.Killed)
        {
            Log.Add($"FACT  {current.Target} died (killer {current.Source})");
            Publish(new DeathFact(ctx, current.Target, current.Source), ctx);
        }
    }

    void Drop(Request r, string reason, FactCtx ctx)
    {
        Log.Add($"DROP  {r.Id} {reason}");
        Publish(new RequestDropped(ctx, reason), ctx);
    }

    // ---------------- resolve: statuses ----------------
    static long Merge(MergeOp op, long existing, long incoming) => op switch
    {
        MergeOp.Keep => existing,
        MergeOp.Replace => incoming,
        MergeOp.Add => existing + incoming,
        MergeOp.Subtract => existing - incoming,
        MergeOp.Max => Math.Max(existing, incoming),
        _ => existing,
    };

    void ResolveStatus(StatusRequest r)
    {
        var ctx = CtxOf(r);
        var owner = _entities.Find(r.Target);
        if (owner is null) { Drop(r, "target gone", ctx); return; }
        if (r.Op == StatusOp.Remove)
        {
            foreach (var inst in StatusesOf(r.Target).Where(i => r.RemoveFilter.MatchesForRemoval(i.Def)).OrderBy(i => i.Id).ToList())
                RemoveInstance(inst, r.Reason, ctx);
            return;
        }

        var def = r.Def ?? throw new InvalidOperationException("Apply/Adjust needs a definition.");
        var inst2 = _statuses.TryGetValue(r.Target, out var targetStatuses) ? targetStatuses.Find(def.Id) : null;
        long inLevel = r.Level.Resolve();
        long? inDur = r.HasDuration ? r.Duration.Resolve() : null;

        if (inst2 is null)
        {
            if (r.Op != StatusOp.Apply) { Log.Add($"NOOP  adjust on missing status {def.Name}"); return; }
            int level = def.Level is null ? 1 : (int)Math.Clamp(inLevel, 0, def.Level.Max);
            if (level == 0) { Log.Add($"NOOP  level 0, {def.Name} not created"); return; }
            long? expires = def.Time is not null ? Now + (inDur ?? 0) : null;
            Create(def, r, owner, level, expires, ctx);
            return;
        }

        if (r.Op == StatusOp.Apply) inst2.Def.OnReapply(inst2);   // typed data may refresh itself, even if level and time do not change

        var levelOp = r.LevelOp ?? (r.Op == StatusOp.Apply ? def.Level?.OnApply : null) ?? MergeOp.Keep;
        var timeOp = r.TimeOp ?? (r.Op == StatusOp.Apply ? def.Time?.OnApply : null) ?? MergeOp.Keep;
        int newLevel = def.Level is null ? inst2.Level : (int)Math.Clamp(Merge(levelOp, inst2.Level, inLevel), 0, def.Level.Max);
        long? newExp = def.Time is not null && inDur is { } d
            ? (long)(timeOp == MergeOp.Replace || timeOp == MergeOp.Max ? Merge(timeOp, (long)(inst2.ExpiresAt ?? Now), (long)(Now + d)) : Merge(timeOp, (long)(inst2.ExpiresAt ?? Now), (long)d))
            : inst2.ExpiresAt;

        if (newLevel == inst2.Level && newExp == inst2.ExpiresAt) { Log.Add($"NOOP  {def.Name} unchanged"); return; }

        int lvlBefore = inst2.Level; long? expBefore = inst2.ExpiresAt;
        inst2.Level = newLevel; inst2.ExpiresAt = newExp; inst2.Applier = r.Source;
        if (newLevel != lvlBefore) foreach (var h in inst2.EffectHandles) h.Reweight(inst2.Weight);
        Log.Add($"FACT  {def.Name} on {r.Target}: level {lvlBefore} -> {newLevel}, expires {expBefore?.ToString() ?? "-"} -> {newExp?.ToString() ?? "-"}");
        Publish(new StatusChanged(ctx, r.Target, def.Id, inst2.Id, lvlBefore, newLevel, expBefore, newExp), ctx);

        if (def.Level is { EndsAtZero: true } && newLevel == 0) RemoveInstance(inst2, RemoveReason.ZeroLevel, ctx);
        else if (newExp is { } t && t <= Now) RemoveInstance(inst2, RemoveReason.Expired, ctx);
    }

    void Create(StatusDefinition def, StatusRequest r, ICombatEntity owner, int level, long? expires, FactCtx ctx)
    {
        var inst = new StatusInstance(++_nextStatus, def, r.Target, level, expires, r.Source);
        inst.Source = Spawn(EntitySource(r.Target), def.Name, SourceTagSet.Of(SourceTags.Status));
        inst.Data = def.CreateData(inst, owner);   // before effects/install, so they can read it
        if (def.Effects is not null)
            foreach (var e in def.Effects) inst.EffectHandles.Add(e.Apply(inst, owner));
        if (def.Install is not null) inst.Handles.AddRange(def.Install(inst, this));

        var registry = StatusRegistryOf(r.Target);
        registry.Register(inst);
        Log.Add($"FACT  {def.Name} applied on {r.Target}: level {level}, expires {expires?.ToString() ?? "-"}");
        Publish(new StatusApplied(ctx, r.Target, def.Id, inst.Id, level, expires, r.Source), ctx);
    }

    void RemoveInstance(StatusInstance inst, RemoveReason reason, FactCtx ctx)
    {
        if (inst.Removed) return;
        inst.Removed = true;
        foreach (var h in inst.EffectHandles) h.Dispose();
        foreach (var h in inst.Handles) h.Dispose();
        (inst.Data as IDisposable)?.Dispose();
        if (_statuses.TryGetValue(inst.Owner, out var owned)) owned.Remove(inst);
        Terminate(inst.Source, by: Root);   // graceful: reactions still in flight keep their chain until they are done
        Log.Add($"FACT  {inst.Def.Name} removed from {inst.Owner} ({reason})");
        Publish(new StatusRemoved(ctx, inst.Owner, inst.Def.Id, inst.Id, reason), ctx);
    }
}
