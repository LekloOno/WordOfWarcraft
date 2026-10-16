using System.Collections.Immutable;
using CombatCore.Primitives;

namespace CombatCore;

/// <summary>
/// Resolves requests through fixed stages, one at a time, FIFO (anything emitted while resolving is queued):
///   fuse -> replace -> add -> modify -> resolve -> publish fact (reactions enqueue derived requests).
/// The stages are shared by every request type; only 'resolve' is type specific.
/// </summary>
public sealed class Authority(IGameClock clock, IEntityDirectory entities)
{
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
    readonly Dictionary<EntityId, List<StatusInstance>> _statuses = [];
    readonly Queue<Request> _queue = new();
    readonly PipelineSet _pipelines = new();
    readonly Dictionary<Type, IFactChannel> _facts = [];
    readonly Dictionary<InstanceId, SpellInstance> _instances = [];
    uint _nextRequest, _nextStatus, _nextInstance;
    long _nextTimestamp;
    bool _running;

    // ---------------- queries ----------------
    public IReadOnlyList<StatusInstance> StatusesOf(EntityId e) =>
        _statuses.TryGetValue(e, out var l) ? l : Array.Empty<StatusInstance>();
    public bool HasTag(EntityId e, Tag t) => StatusesOf(e).Any(s => s.Def.Grants.Contains(t));

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
        Enqueue(draft, Origin.Original, ImmutableArray<uint>.Empty, 0);
        if (!_running) Run();
    }

    /// <summary>Call once per tick. Reads the clock and expires due statuses (through the pipeline, as Remove requests).</summary>
    public void Update()
    {
        Now = clock.Tick;
        var due = _statuses.Values.SelectMany(l => l)
            .Where(s => s.ExpiresAt is { } t && t <= Now)
            .OrderBy(s => s.ExpiresAt).ThenBy(s => s.Id).ToList();
        foreach (var s in due)
            Submit(new StatusDraft(new Source(SourceKind.System, 0, s.Owner), s.Owner, StatusOp.Remove,
                                   RemoveFilter: new StatusFilter(DefId: s.Def.Id), Reason: RemoveReason.Expired));
    }

    // ---------------- spell lifecycle ----------------
    public SpellInstance StartInstance(uint abilityId, EntityId caster, InstanceId? parent = null)
    {
        var inst = new SpellInstance(this, new InstanceId(++_nextInstance), abilityId, caster, parent);
        _instances[inst.Id] = inst;
        Log.Add($"LIFE  {inst.Id} started: ability {abilityId} by {caster}");
        PublishLifecycle(new InstanceStarted(inst.Id, abilityId, caster, parent), inst.Id);
        return inst;
    }

    internal void StopInstance(SpellInstance inst, StopReason reason)
    {
        if (inst.Stopped) return;
        inst.Stopped = true;
        _instances.Remove(inst.Id);
        Log.Add($"LIFE  {inst.Id} stopped: {reason}");
        PublishLifecycle(new InstanceStopped(inst.Id, inst.AbilityId, inst.Caster, reason), inst.Id);
    }

    void PublishLifecycle<T>(T fact, InstanceId cause)
    {
        Publish(fact, new FactCtx(default, cause, ImmutableArray<uint>.Empty, 0));
        if (!_running) Run();
    }

    /// <summary>
    /// Call when an entity is about to be freed: ends every status it owns (their effects are disposed).
    /// Requests still queued for it are dropped when they reach resolution (the directory no longer finds it).
    /// </summary>
    public void RemoveEntity(EntityId entity)
    {
        var ctx = new FactCtx(default, null, ImmutableArray<uint>.Empty, 0);
        foreach (var inst in StatusesOf(entity).ToList()) RemoveInstance(inst, RemoveReason.OwnerRemoved, ctx);
        _statuses.Remove(entity);
    }

    void Enqueue(Draft d, Origin origin, ImmutableArray<uint> lineage, int depth)
    {
        var id = new RequestId(++_nextRequest);
        _queue.Enqueue(d switch
        {
            RequestDraft r => new ResourceRequest(id, r.Source, r.Cause, origin, lineage, depth, r.Target,
                                                  r.Resource, r.Role, r.Tags, new BucketedValue(r.Amount)),
            StatusDraft s => new StatusRequest(id, s.Source, s.Cause, origin, lineage, depth, s.Target,
                                               s.Op, s.Def, new BucketedValue(s.Level), new BucketedValue((long)(s.Duration ?? 0)),
                                               s.Duration.HasValue, s.LevelOp, s.TimeOp, s.RemoveFilter, s.Reason),
            _ => throw new NotSupportedException(d.GetType().Name),
        });
    }

    void Run()
    {
        _running = true;
        try
        {
            while (_queue.TryDequeue(out var r))
                switch (r)
                {
                    case ResourceRequest rr: Process(rr, ResolveResource); break;
                    case StatusRequest sr: Process(sr, ResolveStatus); break;
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
        Log.Add($"REQ   {original.Describe()}");
        var p = P<T>();

        // 2. replace: one pass ordered by (phase, registration time); each sees the previous output;
        //    an interceptor never acts twice in one lineage.
        var current = (T)original.CloneValues();
        foreach (var e in p.Replacements.OrderBy(x => x.Item.Phase).ThenBy(x => x.Timestamp).ToList())
        {
            var rep = e.Item;
            if (current.Lineage.Contains(rep.Id) || !rep.Match(current)) continue;
            current = (T)rep.Apply(current).WithLineage(current.Lineage.Add(rep.Id));
            Log.Add($"REPL  {rep.Id}");
        }

        // 3. add: decided on the ORIGINAL request, queued, never resolved inline.
        foreach (var e in p.Additions.OrderBy(x => x.Timestamp).ToList())
        {
            var add = e.Item;
            if (original.Lineage.Contains(add.Id) || !add.Match(original)) continue;
            foreach (var d in add.Add(original))
            {
                Log.Add($"ADD   {add.Id}");
                Enqueue(d with { Cause = d.Cause ?? original.Cause }, Origin.Derived, original.Lineage.Add(add.Id), original.Depth + 1);
            }
        }

        // 4. modify: on the final shape of the request, derived or not (unless the filter says otherwise).
        foreach (var e in p.Modifiers.OrderBy(x => x.Timestamp).ToList())
        {
            var mod = e.Item;
            if (!mod.Match(current)) continue;
            int before = current.ContributionCount;
            mod.Apply(current, mod.Id);
            if (current.ContributionCount > before) Log.Add($"MOD   {mod.Id}");
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

    // ---------------- resolve: resources ----------------
    void ResolveResource(ResourceRequest current)
    {
        var ctx = CtxOf(current);
        var entity = entities.Find(current.Target);
        if (entity is null) { Drop(current, "target gone", ctx); return; }
        if (!entity.Resources.Has(current.Resource)) { Log.Add($"NOOP  {current.Id} {current.Target} has no resource {current.Resource.Value}"); return; }

        long amount = current.Value.Resolve();
        bool removes = (current.Role & Role.Drain) != 0;
        var change = entity.Resources.Apply(current.Resource, removes ? ResourceDirection.Remove : ResourceDirection.Add, amount);

        var fact = new ResourceFact(ctx, current.Source, current.Target, current.Resource, current.Role, current.Tags,
                                    amount, change.Applied, change.Before, change.After, current.Value.Contributions.ToArray());
        Log.Add($"FACT  {current.Id} {current.Role} res{current.Resource.Value} {current.Target}: {change.Before} -> {change.After}"
                + (fact.Contributions.Count > 0 ? $"  [base {current.Value.Base}, " + string.Join(", ", fact.Contributions.Select(c => $"{c.By}:{c.Kind}{c.Amount:+0.##;-0.##}")) + "]" : ""));
        Publish(fact, ctx);

        if (change.Killed)
        {
            Log.Add($"FACT  {current.Target} died (killer {current.Source.Kind}:{current.Source.Id})");
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
        var owner = entities.Find(r.Target);
        if (owner is null) { Drop(r, "target gone", ctx); return; }
        if (r.Op == StatusOp.Remove)
        {
            foreach (var inst in StatusesOf(r.Target).Where(i => r.RemoveFilter.MatchesForRemoval(i.Def)).OrderBy(i => i.Id).ToList())
                RemoveInstance(inst, r.Reason, ctx);
            return;
        }

        var def = r.Def ?? throw new InvalidOperationException("Apply/Adjust needs a definition.");
        var inst2 = StatusesOf(r.Target).FirstOrDefault(i => i.Def.Id == def.Id);
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
        if (def.Effects is not null)
            foreach (var e in def.Effects) inst.EffectHandles.Add(e.Apply(inst, owner));
        if (def.Install is not null) inst.Handles.AddRange(def.Install(inst, this));

        if (!_statuses.TryGetValue(r.Target, out var list)) _statuses[r.Target] = list = new();
        list.Add(inst);
        Log.Add($"FACT  {def.Name} applied on {r.Target}: level {level}, expires {expires?.ToString() ?? "-"}");
        Publish(new StatusApplied(ctx, r.Target, def.Id, inst.Id, level, expires, r.Source), ctx);
    }

    void RemoveInstance(StatusInstance inst, RemoveReason reason, FactCtx ctx)
    {
        if (inst.Removed) return;
        inst.Removed = true;
        foreach (var h in inst.EffectHandles) h.Dispose();
        foreach (var h in inst.Handles) h.Dispose();
        if (_statuses.TryGetValue(inst.Owner, out var owned)) owned.Remove(inst);
        Log.Add($"FACT  {inst.Def.Name} removed from {inst.Owner} ({reason})");
        Publish(new StatusRemoved(ctx, inst.Owner, inst.Def.Id, inst.Id, reason), ctx);
    }
}
