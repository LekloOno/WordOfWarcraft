using CombatCore;
using CombatCore.Primitives;
using CombatCore.Primitives.Tags;
using CombatCore.Sources;
using CombatCore.Sources.Lifecycle;
using ReferenceSpells;
using CombatCore.Tests;

int failed = 0, passed = 0;
void Check(string name, bool ok) { if (ok) passed++; else failed++; Console.WriteLine($"{(ok ? "PASS" : "FAIL")}  {name}"); }
void Show(Authority a, string title) { Console.WriteLine($"\n--- {title}"); foreach (var l in a.Log) Console.WriteLine("  " + l); Console.WriteLine(); }

var Mage = new EntityId(1); var B = new EntityId(2); var C = new EntityId(3); var X = new EntityId(4);

// A fresh authority over a fake world. Every entity starts with empty health/mana/stamina pools.
// Sources belong to an authority (they are nodes of its tree), so the helpers below read the authority of the running scenario.
Authority cur = null!;
SpellBook book = null!;
Authority New(out FakeWorld w, out ManualClock clock, int maxDepth = 8)
{
    var world = new FakeWorld(); clock = new ManualClock();
    foreach (var e in new[] { Mage, B, C, X })
    { world.Add(e); world.Set(e, R.Health, 0); world.Set(e, R.Mana, 0); world.Set(e, R.Stamina, 0); }
    w = world;
    cur = new Authority(clock, world) { MaxDepth = maxDepth };
    book = new SpellBook(cur);
    return cur;
}
void At(Authority a, ManualClock c, long tick) { c.Tick = tick; a.Update(); }
Source Spell(uint id) => book.Ability(Mage, id);                            // Entity(Mage) > Ability id
var statusNodes = new Dictionary<(Authority, uint), Source>();
Source Status(uint id)                                                      // a status-tagged node under Mage, one per id
{
    if (!statusNodes.TryGetValue((cur, id), out var n))
        statusNodes[(cur, id)] = n = cur.Spawn(cur.EntitySource(Mage), $"Status{id}", SourceTagSet.Of(SourceTags.Status));
    return n;
}
Source Sys(EntityId owner) => cur.Root;
IEnumerable<Draft> None() => Array.Empty<Draft>();

// =====================  part 1: pipeline (unchanged behavior)  =====================

{   // drain becomes damage, then +5
    var a = New(out var w, out var clock);
    w.Set(B, R.Health, 100); w.Set(B, R.Mana, 100);
    a.Add(Rules.Replace(new(direction: ResourceDirection.Remove), Phase.Convert, r => r with { Direction = ResourceDirection.Remove, Resource = R.Health }));
    a.Add(Rules.Mod(new(direction: ResourceDirection.Remove), ValueMod.Plus(5)));
    a.Submit(new ResourceDraft(Spell(1), B, R.Mana, ResourceDirection.Remove, 20));
    Check("P1 drain -> damage +5: health 75, mana untouched", w.Get(B, R.Health) == 75 && w.Get(B, R.Mana) == 100);
}
{   // competing replacements
    var a = New(out var w, out var clock);
    w.Set(B, R.Health, 100); w.Set(B, R.Stamina, 100); w.Set(B, R.Mana, 100);
    var drain = RequestTag.Register("test.drain"); var dmg = RequestTag.Register("test.damage");
    // both match "drain"; the first converts it (rewrites the tags), so the second no longer matches
    a.Add(Rules.Replace(new(RequireAll: RequestTagSet.Of(drain)), Phase.Convert, r => r with { Resource = R.Health, Tags = RequestTagSet.Of(dmg) }));
    a.Add(Rules.Replace(new(RequireAll: RequestTagSet.Of(drain)), Phase.Convert, r => r with { Resource = R.Stamina, Tags = RequestTagSet.Of(dmg) }));
    a.Submit(new ResourceDraft(Spell(1), B, R.Mana, ResourceDirection.Remove, 20, RequestTagSet.Of(drain)));
    Check("P2 first replacement wins", w.Get(B, R.Health) == 80 && w.Get(B, R.Stamina) == 100);
}
{   // lineage rule
    var a = New(out var w, out var clock);
    w.Set(B, R.Health, 1000); w.Set(B, R.Mana, 1000);
    var drain = RequestTag.Register("test.drain"); var dmg = RequestTag.Register("test.damage");
    a.Add(Rules.AddOn(new(RequireAll: RequestTagSet.Of(dmg)),   r => new[] { new ResourceDraft(Status(1), B, R.Mana, ResourceDirection.Remove, 5, RequestTagSet.Of(drain)) }));
    a.Add(Rules.AddOn(new(RequireAll: RequestTagSet.Of(drain)), r => new[] { new ResourceDraft(Status(2), B, R.Health, ResourceDirection.Remove, 5, RequestTagSet.Of(dmg)) }));
    a.Submit(new ResourceDraft(Spell(1), B, R.Health, ResourceDirection.Remove, 10, RequestTagSet.Of(dmg)));
    Check("P3 lineage: terminates after one round, nothing dropped",
          w.Get(B, R.Health) == 985 && w.Get(B, R.Mana) == 995 && !a.Log.Any(l => l.StartsWith("DROP")));
}
{   // depth fuse
    var a = New(out var w, out var clock, maxDepth: 4);
    w.Set(B, R.Health, 1000);
    a.OnFact<ResourceFact>(f => new[] { new ResourceDraft(Status(9), B, R.Health, ResourceDirection.Remove, 1) });
    a.Submit(new ResourceDraft(Spell(1), B, R.Health, ResourceDirection.Remove, 1));
    Check("P4 depth fuse", w.Get(B, R.Health) == 995 && a.Log.Any(l => l.StartsWith("DROP")));
}
{   // direct vs derived discrimination
    var a = New(out var w, out var clock);
    w.Set(B, R.Health, 100); w.Set(C, R.Health, 100);
    a.Add(Rules.Mod(new(direction: ResourceDirection.Remove, Source: new(RequireAll: SourceTagSet.Of(SpellTags.Spell)), Origin: OriginMask.Original), ValueMod.Plus(5)));
    a.Add(Rules.Mod(new(direction: ResourceDirection.Remove), ValueMod.Plus(1)));
    a.OnFact<ResourceFact>(f => f.Source.InheritedTags.Contains(SpellTags.Spell) && f.Direction == ResourceDirection.Remove
        ? new[] { new ResourceDraft(Status(7), C, R.Health, ResourceDirection.Remove, 10) } : None());
    a.Submit(new ResourceDraft(Spell(1), B, R.Health, ResourceDirection.Remove, 20));
    Check("P5 direct-only modifier skips derived damage", w.Get(B, R.Health) == 74 && w.Get(C, R.Health) == 89);
}
{   // rollback: refund on kill reads the cost fact
    foreach (var free in new[] { false, true })
    {
        var costTag = RequestTag.Register("cost");
        var tagSet = RequestTagSet.Of(costTag);
        var a = New(out var w, out var clock);
        var optedIn = new HashSet<uint> { 1 };
        var spent = new Dictionary<Source, long>();
        w.Set(Mage, R.Mana, 100); w.Set(X, R.Health, 10);
        if (free) a.Add(Rules.Mod(new(direction: ResourceDirection.Remove, Source: new(Under: Spell(1))), ValueMod.Times(0)));
        a.OnFact<ResourceFact>((Func<ResourceFact, IEnumerable<Draft>>)(f => { if (f.Direction == ResourceDirection.Remove && f.Ctx.Cause is { } c && f.Tags.ContainsAll(tagSet)) spent[c] = spent.GetValueOrDefault(c) + f.Applied; return None(); }));
        a.OnFact<DeathFact>(d => d.Ctx.Cause is { } c && c.FindAttachment<IAbilityAttachment>()?.AbilityId is { } ab && optedIn.Contains(ab) && spent.GetValueOrDefault(c) > 0
            ? new Draft[] { new ResourceDraft(Status(99), Mage, R.Mana, ResourceDirection.Add, spent[c], default, c) } : None());
        var inst = book.Start(1, Mage).Node;     // the run: its ability is found by climbing the tree
        a.Submit(new ResourceDraft(inst, Mage, R.Mana, ResourceDirection.Remove, 30, tagSet, inst));
        a.Submit(new ResourceDraft(Status(5), X, R.Health, ResourceDirection.Remove, 40, default, inst));
        Check($"P6 rollback (free={free}): mana back to 100, refund {(free ? "skipped" : "emitted")}",
              w.Get(Mage, R.Mana) == 100 && a.Log.Any(l => l.Contains("REACT DeathFact")) == !free);
    }
}

// =====================  part 2: statuses  =====================

var Strength = new StatId(1);
StatusDefinition Might(StatusCategory cat = StatusCategory.Buff) => new(
    "Might", cat, Level: new LevelModel(10, MergeOp.Add), Time: new TimeModel(MergeOp.Replace),
    Effects: new IStatusEffect[] { new StatEffect(new StatContribution(Strength, Flat: 10)) });

{   // S1: level merge (Add), stat scales with weight, a level change notifies ONCE (Reweight)
    var a = New(out var w, out var clock); var d = Might();
    a.Submit(new StatusDraft(Spell(1), B, StatusOp.Apply, d, Level: 2, Duration: 100));
    int n1 = w.Notifications(B);
    a.Submit(new StatusDraft(Spell(1), B, StatusOp.Apply, d, Level: 3, Duration: 100));
    Show(a, "S1: apply level 2, then level 3 (Add)");
    Check("S1 level 2 -> stat +20", a.StatusesOf(B).First().Level == 5 && w.Stat(B, Strength, 0) == 50);
    Check("S1 reweight notified once (not dispose + add)", w.Notifications(B) - n1 == 1);
}
{   // S2: time merge (Replace = refresh), expiry through the pipeline, stat removed with the status
    var a = New(out var w, out var clock); var d = Might();
    At(a, clock, 0);
    a.Submit(new StatusDraft(Spell(1), B, StatusOp.Apply, d, Level: 1, Duration: 100));
    At(a, clock, 10);
    a.Submit(new StatusDraft(Spell(1), B, StatusOp.Apply, d, Level: 1, Duration: 50));   // refresh: expires at 60
    At(a, clock, 59);
    Check("S2 alive at 59 (refreshed), level 2", a.StatusesOf(B).Count == 1 && a.StatusesOf(B).First().Level == 2);
    At(a, clock, 60);
    Check("S2 expired at 60, stat gone", a.StatusesOf(B).Count == 0 && w.Stat(B, Strength, 0) == 0);
    Check("S2 removal fact says Expired", a.Log.Any(l => l.Contains("removed") && l.Contains("Expired")));
}
{   // S3: time op Max keeps the longer one
    var a = New(out var w, out var clock);
    var d = Might() with { Time = new TimeModel(MergeOp.Max) };
    At(a, clock, 0);
    a.Submit(new StatusDraft(Spell(1), B, StatusOp.Apply, d, Level: 1, Duration: 100));
    At(a, clock, 10);
    a.Submit(new StatusDraft(Spell(1), B, StatusOp.Apply, d, Level: 1, Duration: 20));   // 30 < 100: keep 100
    Check("S3 Max keeps the longest expiry", a.StatusesOf(B).First().ExpiresAt == 100);
}
{   // S4: FreeNextCast: charge consumed by a reaction, cost stat visible BEFORE the cast, removal restores it
    var a = New(out var w, out var clock);
    var Cost = new StatId(1001);
    StatusDefinition? free = null;
    free = new StatusDefinition("FreeNextCast", StatusCategory.Buff,
        Level: new LevelModel(1, MergeOp.Replace, EndsAtZero: true),
        Effects: new IStatusEffect[] { new StatEffect(new StatContribution(Cost, Mult: 0f)) },
        Install: (inst, auth) => new[] { auth.OnFact<ResourceFact>(f =>
            f.Direction == ResourceDirection.Remove && f.Source.IsSameOrDescendantOf(Spell(1)) && f.Source.OwnerOf() == inst.Owner
                ? new Draft[] { new StatusDraft(inst.Source, inst.Owner, StatusOp.Adjust, free, Level: 1, LevelOp: MergeOp.Subtract) }
                : None()) });
    w.Set(Mage, R.Mana, 100);
    Check("S4 cost stat 30 before", w.Stat(Mage, Cost, 30) == 30);
    a.Submit(new StatusDraft(Spell(2), Mage, StatusOp.Apply, free, Level: 1));
    int effective = w.Stat(Mage, Cost, 30);
    Check("S4 UI/precondition read effective cost 0", effective == 0);
    a.Submit(new ResourceDraft(Spell(1), Mage, R.Mana, ResourceDirection.Remove, effective));     // the cast spends the effective cost
    Show(a, "S4: free cast consumes its charge");
    Check("S4 spent 0, status ended (ZeroLevel), cost back to 30",
          w.Get(Mage, R.Mana) == 100 && a.StatusesOf(Mage).Count == 0 && w.Stat(Mage, Cost, 30) == 30);
}
{   // S5: an interceptor scales with the LIVE level, no re-registration
    var a = New(out var w, out var clock);
    w.Set(B, R.Health, 1000);
    var weakened = new StatusDefinition("Weakened", StatusCategory.Debuff,
        Level: new LevelModel(5, MergeOp.Add),
        Install: (inst, auth) => new[] { auth.Add(Rules.Mod(
            new(direction: ResourceDirection.Remove, Source: new(Owner: inst.Owner)), _ => ValueMod.Times(1 - 0.1 * inst.Weight))) });
    a.Submit(new StatusDraft(Status(9), B, StatusOp.Apply, weakened, Level: 1));
    a.Submit(new ResourceDraft(book.Ability(B, 1), C, R.Health, ResourceDirection.Remove, 100));
    w.Set(C, R.Health, 1000);
    a.Submit(new ResourceDraft(book.Ability(B, 1), C, R.Health, ResourceDirection.Remove, 100));
    long afterL1 = w.Get(C, R.Health);
    a.Submit(new StatusDraft(Status(9), B, StatusOp.Apply, weakened, Level: 1));        // level 2
    w.Set(C, R.Health, 1000);
    a.Submit(new ResourceDraft(book.Ability(B, 1), C, R.Health, ResourceDirection.Remove, 100));
    Check("S5 level 1: 100 -> 90, level 2: 100 -> 80", afterL1 == 910 && w.Get(C, R.Health) == 920);
}
{   // S6: cleanse by category; passives are never swept by a catch-all
    var a = New(out var w, out var clock);
    var Slowed = GrantTag.Register("Slowed");
    var buff = Might(); 
    var debuff = new StatusDefinition("Slow", StatusCategory.Debuff, Grants: GrantTagSet.Of(Slowed));
    var passive = new StatusDefinition("Ward", StatusCategory.Passive);
    foreach (var d in new[] { buff, debuff, passive })
        a.Submit(new StatusDraft(Spell(1), B, StatusOp.Apply, d, Level: 1, Duration: 1000));
    Check("S6 tag present while Slow is active", a.HasTag(B, Slowed));
    a.Submit(new StatusDraft(Spell(3), B, StatusOp.Remove, RemoveFilter: new StatusFilter(StatusCategory.Debuff)));
    Check("S6 cleanse(Debuff) removes only the debuff, tag gone", a.StatusesOf(B).Count == 2 && !a.HasTag(B, Slowed));
    a.Submit(new StatusDraft(Spell(3), B, StatusOp.Remove));                           // catch-all
    Check("S6 catch-all removes the buff, keeps the passive", a.StatusesOf(B).Count == 1 && a.StatusesOf(B).First().Def.Name == "Ward");
}
{   // S7: status acting on statuses: +20% level on received buffs
    var a = New(out var w, out var clock);
    var blessed = new StatusDefinition("Blessed", StatusCategory.Passive,
        Install: (inst, auth) => new[] { auth.Add(Rules.LevelMod(
            new(new StatusFilter(StatusCategory.Buff), StatusOp.Apply, Target: inst.Owner), ValueMod.Times(1.2))) });
    a.Submit(new StatusDraft(Sys(B), B, StatusOp.Apply, blessed));
    a.Submit(new StatusDraft(Spell(1), B, StatusOp.Apply, Might(), Level: 5, Duration: 100));
    Show(a, "S7: Blessed boosts received buffs");
    var might = a.StatusesOf(B).First(s => s.Def.Name == "Might");
    Check("S7 level 5 * 1.2 = 6, stat +60", might.Level == 6 && w.Stat(B, Strength, 0) == 60);
    Check("S7 passive itself not boosted (level untouched)", a.StatusesOf(B).First(s => s.Def.Name == "Blessed").Level == 1);
}
{   // S8: disposing the status removes every effect it installed
    var a = New(out var w, out var clock); w.Set(B, R.Health, 100);
    var d = new StatusDefinition("Sting", StatusCategory.Neutral, Time: new TimeModel(MergeOp.Replace),
        Install: (inst, auth) => new[] { auth.Add(Rules.Mod(new(direction: ResourceDirection.Remove, Target: inst.Owner), ValueMod.Plus(5))) });
    At(a, clock, 0);
    a.Submit(new StatusDraft(Spell(1), B, StatusOp.Apply, d, Duration: 10));
    a.Submit(new ResourceDraft(Spell(1), B, R.Health, ResourceDirection.Remove, 10));
    At(a, clock, 10);
    a.Submit(new ResourceDraft(Spell(1), B, R.Health, ResourceDirection.Remove, 10));
    Check("S8 +5 while active (15), gone after expiry (10)", w.Get(B, R.Health) == 75);
}

// =====================  part 3: spell lifecycle  =====================

{   // L1: the spell asks for an instance, emits through it, stops once with a reason
    var a = New(out var w, out var clock);
    w.Set(B, R.Health, 100); w.Set(Mage, R.Mana, 100);
    var started = new List<InstanceStarted>(); var stopped = new List<InstanceStopped>(); var facts = new List<ResourceFact>();
    var climbed = new List<(uint? Ability, EntityId? Owner)>();   // what a handler sees WHILE the fact is published
    a.OnFact<InstanceStarted>(f => { started.Add(f); return None(); });
    a.OnFact<InstanceStopped>(f => { stopped.Add(f); return None(); });
    a.OnFact<ResourceFact>(f => { facts.Add(f); climbed.Add((f.Source.FindAttachment<IAbilityAttachment>()?.AbilityId, f.Source.OwnerOf())); return None(); });

    var inst = book.Start(abilityId: 7, caster: Mage);
    inst.Request(Mage, R.Mana, ResourceDirection.Remove, 30);
    inst.Request(B, R.Health, ResourceDirection.Remove, 20);
    inst.Stop(StopReason.Completed);
    inst.Stop(StopReason.Cancelled);                      // ignored: a flow ends once
    Show(a, "L1: spell lifecycle");
    Check("L1 started and stopped once, with the first reason", started.Count == 1 && stopped.Count == 1 && stopped[0].Reason == StopReason.Completed);
    Check("L1 every fact carries the instance node as cause and source (ability and owner found by climbing)",
          facts.Count == 2 && facts.All(f => f.Ctx.Cause == inst.Node && f.Source == inst.Node) && climbed.All(c => c.Ability == 7 && c.Owner == Mage));
    Check("L1 effects applied", w.Get(Mage, R.Mana) == 70 && w.Get(B, R.Health) == 80);
}
{   // L2: consequences outlive the flow: the instance stays pending while a child node (a projectile) is in flight
    var a = New(out var w, out var clock);
    w.Set(B, R.Health, 100);
    var inst = book.Start(7, Mage);
    var hold = new Hold();
    var projectile = a.Spawn(inst.Node, "projectile", default, hold);
    inst.Stop(StopReason.Completed);
    Check("L2 flow stopped, node pending (the projectile holds it)", inst.Stopped && inst.Node.State == SourceState.PendingTermination && projectile.State == SourceState.PendingTermination);
    a.Submit(new ResourceDraft(projectile, B, R.Health, ResourceDirection.Remove, 10, default, inst.Node));
    Check("L2 the projectile lands after the flow ended", w.Get(B, R.Health) == 90);
    hold.Ticket.Complete();
    Check("L2 landed: the whole run is gone", inst.Node.State == SourceState.Terminated && projectile.State == SourceState.Terminated);
    int before = a.Log.Count(l => l.StartsWith("DROP"));
    inst.Request(B, R.Health, ResourceDirection.Remove, 10);
    Check("L2 a finished run can not emit anymore (dropped, B untouched)", w.Get(B, R.Health) == 90 && a.Log.Count(l => l.StartsWith("DROP")) == before + 1);
}
{   // L3: a reaction to a lifecycle fact (the hook you want to bind things on)
    var a = New(out var w, out var clock);
    w.Set(Mage, R.Mana, 50);
    a.OnFact<InstanceStopped>(f => f.Reason == StopReason.Interrupted
        ? new Draft[] { new ResourceDraft(Status(1), f.Caster, R.Mana, ResourceDirection.Add, 10) } : None());
    book.Start(7, Mage).Stop(StopReason.Completed);
    Check("L3 no reaction on Completed", w.Get(Mage, R.Mana) == 50);
    book.Start(7, Mage).Stop(StopReason.Interrupted);
    Check("L3 reaction on Interrupted refunds", w.Get(Mage, R.Mana) == 60);
}

// =====================  part 4: entity removal  =====================

{   // E1: removing an entity ends its statuses (effects disposed) and later requests to it are dropped
    var a = New(out var w, out var clock);
    var Strength2 = new StatId(2);
    var buff = new StatusDefinition("Buff", StatusCategory.Buff,
        Effects: new IStatusEffect[] { new StatEffect(new StatContribution(Strength2, Flat: 5)) });
    a.Submit(new StatusDraft(Spell(1), B, StatusOp.Apply, buff));
    var entityB = w[B];
    Check("E1 buff active", entityB.Stat(Strength2, 0) == 5);
    var removed = new List<StatusRemoved>();
    a.OnFact<StatusRemoved>(f => { removed.Add(f); return None(); });
    a.RemoveEntity(B);
    w.Remove(B);
    Check("E1 status removed with reason OwnerRemoved, effect disposed",
          removed.Count == 1 && removed[0].Reason == RemoveReason.OwnerRemoved && entityB.Stat(Strength2, 0) == 0);
    a.Submit(new ResourceDraft(Spell(1), B, R.Health, ResourceDirection.Remove, 10));
    Check("E1 request to a removed entity is dropped", a.Log.Any(l => l.Contains("target gone")));
    Check("E1 the entity node is terminated too (statuses and runs under it with it)", removed.Count == 1 && a.Root.Children.All(n => n.OwnerOf() != B));
}

// =====================  typed statuses (through the usual pipeline)  =====================

{   // T1: created once by the authority, data readable through the definition, reapply hook instead of a new instance
    var a = New(out var w, out var clock);
    int created = 0; FakeBreaches? last = null;
    var applied = new List<StatusApplied>();
    a.OnFact<StatusApplied>(f => { applied.Add(f); return None(); });
    var def = new TypedStatusDefinition<FakeBreaches>("Breaches", StatusCategory.Passive,
        Create: (inst, owner) => { created++; return last = new FakeBreaches(owner.Id); },
        Reapply: (inst, data) => data.Generation++);

    Check("T1 nothing before the apply", !a.TryGetData(Mage, def, out _));
    a.Submit(new StatusDraft(Spell(1), Mage, StatusOp.Apply, def));
    var found = a.TryGetData(Mage, def, out var data);
    Check("T1 data built once, for the right owner, readable by key", found && created == 1 && ReferenceEquals(data, last) && data!.Owner == Mage);
    Check("T1 StatusApplied fact published (the UI and the log see it)", applied.Count == 1 && applied[0].DefId == def.Id);

    a.Submit(new StatusDraft(Spell(1), Mage, StatusOp.Apply, def));
    Check("T1 re-apply: no second instance, reapply hook ran", created == 1 && a.StatusesOf(Mage).Count == 1 && data!.Generation == 2);
}
{   // T2: expiry removes the status through the pipeline, and disposes data + installed handles
    var a = New(out var w, out var clock);
    var flag = new DisposeFlag(); FakeBreaches? made = null;
    var removed = new List<StatusRemoved>();
    a.OnFact<StatusRemoved>(f => { removed.Add(f); return None(); });
    var def = new TypedStatusDefinition<FakeBreaches>("TimedBreaches", StatusCategory.Buff,
        Create: (inst, owner) => made = new FakeBreaches(owner.Id),
        Time: new TimeModel(MergeOp.Replace),
        Install: (inst, auth) => new IDisposable[] { flag });
    At(a, clock, 0);
    a.Submit(new StatusDraft(Spell(1), Mage, StatusOp.Apply, def, Duration: 10));
    At(a, clock, 10);
    Check("T2 expired: data disposed", made is { Disposed: true });
    Check("T2 expired: installed handle disposed", flag.Disposed);
    Check("T2 expired: StatusRemoved(Expired) published, data unreachable",
          removed.Count == 1 && removed[0].Reason == RemoveReason.Expired && !a.TryGetData(Mage, def, out _));
}
{   // T3: cleanse removes typed buffs but a catch-all spares the typed passive; explicit removal and owner removal dispose
    var a = New(out var w, out var clock);
    FakeBreaches? buff = null, passive = null, owned = null;
    var buffDef    = new TypedStatusDefinition<FakeBreaches>("TypedBuff", StatusCategory.Buff,    Create: (i, o) => buff = new FakeBreaches(o.Id));
    var passiveDef = new TypedStatusDefinition<FakeBreaches>("TypedPassive", StatusCategory.Passive, Create: (i, o) => passive = new FakeBreaches(o.Id));
    var ownedDef   = new TypedStatusDefinition<FakeBreaches>("OnC", StatusCategory.Buff,        Create: (i, o) => owned = new FakeBreaches(o.Id));
    a.Submit(new StatusDraft(Spell(1), B, StatusOp.Apply, buffDef));
    a.Submit(new StatusDraft(Spell(1), B, StatusOp.Apply, passiveDef));
    a.Submit(new StatusDraft(Spell(1), C, StatusOp.Apply, ownedDef));
    a.Submit(new StatusDraft(Spell(3), B, StatusOp.Remove));   // catch-all cleanse
    Check("T3 catch-all removes the typed buff and disposes it, spares the passive",
          buff!.Disposed && !passive!.Disposed && a.TryGetData(B, passiveDef, out _) && !a.TryGetData(B, buffDef, out _));
    a.Submit(new StatusDraft(Spell(3), B, StatusOp.Remove, RemoveFilter: new StatusFilter(StatusCategory.Passive)));
    Check("T3 explicit removal by category disposes the passive", passive!.Disposed);
    a.RemoveEntity(C);
    Check("T3 owner removal disposes the data", owned!.Disposed);
}
{   // T4: two definitions sharing a data type do not collide
    var a = New(out var w, out var clock);
    var d1 = new TypedStatusDefinition<FakeBreaches>("A", StatusCategory.Neutral, Create: (i, o) => new FakeBreaches(o.Id));
    var d2 = new TypedStatusDefinition<FakeBreaches>("B", StatusCategory.Neutral, Create: (i, o) => new FakeBreaches(o.Id));
    a.Submit(new StatusDraft(Spell(1), Mage, StatusOp.Apply, d1));
    a.Submit(new StatusDraft(Spell(1), Mage, StatusOp.Apply, d2));
    Check("T4 same data type, two keys, two instances",
          a.TryGetData(Mage, d1, out var x) && a.TryGetData(Mage, d2, out var y) && !ReferenceEquals(x, y));
}

// =====================  part 5: sources (the tree replaces Source kind / id / owner and the instance ids)  =====================

{   // N1: filters read the tree: inherited tags, owner, under a node, exact tags
    var a = New(out var w, out var clock);
    w.Set(B, R.Health, 1000);
    var inst = book.Start(1, Mage);
    var launch = a.Spawn(inst.Node, "launch", SourceTagSet.Of(SourceTag.Register("Launch")));        // Entity > Ability > Instance > Launch
    var dmg = ResourceDirection.Remove;
    a.Add(Rules.Mod(new(direction: dmg, Source: new(RequireAll: SourceTagSet.Of(SpellTags.Spell))), ValueMod.Plus(1)));            // inherited: from a spell        yes
    a.Add(Rules.Mod(new(direction: dmg, Source: new(Under: Spell(2))), ValueMod.Plus(2)));                                          // another ability                no
    a.Add(Rules.Mod(new(direction: dmg, Source: new(Owner: Mage)), ValueMod.Plus(4)));                                              // owner                          yes
    a.Add(Rules.Mod(new(direction: dmg, Source: new(Owner: B)), ValueMod.Plus(8)));                                                // another owner                  no
    a.Add(Rules.Mod(new(direction: dmg, Source: new(Exclude: SourceTagSet.Of(SpellTags.Instance))), ValueMod.Plus(16)));           // inherited Instance excluded    no
    a.Add(Rules.Mod(new(direction: dmg, Source: new(ExactExclude: SourceTagSet.Of(SpellTags.Instance))), ValueMod.Plus(32)));      // the launch itself is no Instance yes
    a.Add(Rules.Mod(new(direction: dmg, Source: new(ExactRequireAll: SourceTagSet.Of(SpellTags.Spell))), ValueMod.Plus(64)));      // the launch itself is no Spell  no
    a.Submit(new ResourceDraft(launch, B, R.Health, ResourceDirection.Remove, 100, default, inst.Node));
    Check("N1 inherited vs exact tags, owner, other ability / other owner", w.Get(B, R.Health) == 1000 - (100 + 1 + 4 + 32));
}
{   // N2: the lifecycle shares the FIFO: a terminate can not overtake a request emitted before it
    var a = New(out var w, out var clock);
    w.Set(B, R.Health, 100);
    var inst = book.Start(1, Mage);
    bool first = true;
    a.OnFact<ResourceFact>(f =>
    {
        if (!first) return None();
        first = false;
        inst.Request(B, R.Health, ResourceDirection.Remove, 5);   // queued
        a.Terminate(inst.Node);                           // queued behind it
        return None();
    });
    a.Submit(new ResourceDraft(Spell(1), B, R.Health, ResourceDirection.Remove, 10));
    Check("N2 the request emitted before the terminate was resolved with its chain intact (no drop), then the node left",
          w.Get(B, R.Health) == 85 && !a.Log.Any(l => l.StartsWith("DROP")) && inst.Node.State == SourceState.Terminated);
}
{   // N3: consequences outlive the entity: removing it cancels its runs, a projectile in flight keeps the chain alive
    var a = New(out var w, out var clock);
    w.Set(B, R.Health, 100);
    var stopped = new List<InstanceStopped>();
    a.OnFact<InstanceStopped>(f => { stopped.Add(f); return None(); });
    var inst = book.Start(7, Mage);
    var hold = new Hold();
    var projectile = a.Spawn(inst.Node, "projectile", default, hold);
    var entityNode = a.EntitySource(Mage);
    a.RemoveEntity(Mage);
    Check("N3 the run is cancelled from outside", stopped.Count == 1 && stopped[0].Reason == StopReason.Cancelled);
    Check("N3 entity, ability, run and projectile are all pending", new[] { entityNode, inst.Node, projectile }.All(n => n.State == SourceState.PendingTermination));
    a.Submit(new ResourceDraft(projectile, B, R.Health, ResourceDirection.Remove, 10, default, inst.Node));
    Check("N3 the projectile still lands, and still knows whose it is", w.Get(B, R.Health) == 90 && projectile.OwnerOf() == Mage);
    hold.Ticket.Complete();
    Check("N3 landed: everything leaves the tree, the entity node is forgotten", new[] { entityNode, inst.Node, projectile }.All(n => n.State == SourceState.Terminated) && a.EntitySource(Mage) != entityNode);
}
{   // N4: a run started under a pending ability is born pending: started, then stopped (cancelled) at once
    var a = New(out var w, out var clock);
    var log = new List<string>();
    a.OnFact<InstanceStarted>(f => { log.Add("started"); return None(); });
    a.OnFact<InstanceStopped>(f => { log.Add("stopped:" + f.Reason); return None(); });
    var ability = book.Ability(Mage, 7);
    a.Spawn(ability, "something in flight", default, new Hold());     // keeps the ability node pending once terminated
    a.Terminate(ability);
    var inst = book.Start(7, Mage);                               // same ability node, now pending
    Check("N4 started then cancelled, in that order (born pending, nothing to wait for)", log.SequenceEqual(new[] { "started", "stopped:Cancelled" }));
    Check("N4 and already gone: its requests would be dropped", inst.Node.State == SourceState.Terminated);
}
{   // N5: lifecycle facts, in order, with the node as cause
    var a = New(out var w, out var clock);
    var seen = new List<string>(); var causes = new List<Source?>();
    a.OnFact<SourceInserted>(f => { seen.Add("inserted " + f.Node.Name); return None(); });
    a.OnFact<SourceTerminating>(f => { seen.Add("terminating " + f.Node.Name); return None(); });
    a.OnFact<SourceTerminated>(f => { seen.Add("terminated " + f.Node.Name + (f.Killed ? " (killed)" : "")); return None(); });
    var hold = new Hold();
    var node = a.Spawn(a.EntitySource(Mage), "thing", default, hold);
    a.Terminate(node);
    seen.Add("-- waiting");
    hold.Ticket.Complete();
    a.Kill(node);   // already terminated: no-op
    Check("N5 inserted, terminating, waiting, terminated",
          seen.SequenceEqual(new[] { "inserted Entity E1", "inserted thing", "terminating thing", "-- waiting", "terminated thing" }));
}
{   // N6: a status instance has a node under its owner; removing the status terminates it
    var a = New(out var w, out var clock);
    var d = new StatusDefinition("Might", StatusCategory.Buff);
    a.Submit(new StatusDraft(Spell(1), B, StatusOp.Apply, d));
    var node = a.StatusesOf(B).First().Source;
    Check("N6 status node: active, tagged, under its owner", node.State == SourceState.Active && node.InheritedTags.Contains(SourceTags.Status) && node.OwnerOf() == B && node.Parent == a.EntitySource(B));
    a.Submit(new StatusDraft(Spell(3), B, StatusOp.Remove));
    Check("N6 removing the status terminates its node", a.StatusesOf(B).Count == 0 && node.State == SourceState.Terminated);
}
{   // N7: an attachment-less node under a pending, awaiting ancestor keeps its chain until the ancestor completes
    var a = New(out var w, out var clock);
    w.Set(B, R.Health, 100);
    var inst = book.Start(1, Mage);
    var hold = new Hold();
    var projectile = a.Spawn(inst.Node, "projectile", default, hold);
    inst.Stop(StopReason.Interrupted);
    var effect = a.Spawn(projectile, "effect holder");                   // no attachment: held by the projectile
    a.Submit(new ResourceDraft(effect, B, R.Health, ResourceDirection.Remove, 10, default, inst.Node));
    Check("N7 the effect holder emits while the projectile is still pending", w.Get(B, R.Health) == 90 && effect.State == SourceState.PendingTermination);
    hold.Ticket.Complete();
    Check("N7 released leaf-first when the projectile completes", effect.State == SourceState.Terminated && inst.Node.State == SourceState.Terminated);
}

{   // N9: exactly this node vs this node or below; the cause is filtered like the source
    var a = New(out var w, out var clock);
    w.Set(B, R.Health, 1000);
    var inst = book.Start(1, Mage);
    var launch = a.Spawn(inst.Node, "launch");
    var dmg = ResourceDirection.Remove;
    a.Add(Rules.Mod(new(direction: dmg, Source: new(Is: inst.Node)), ValueMod.Plus(1)));         // exactly the run: the launch is not it
    a.Add(Rules.Mod(new(direction: dmg, Source: new(Under: inst.Node)), ValueMod.Plus(2)));      // the run or below
    a.Add(Rules.Mod(new(direction: dmg, Cause: new(Is: inst.Node)), ValueMod.Plus(4)));          // the cause is exactly the run
    a.Add(Rules.Mod(new(direction: dmg, Cause: new(Under: Spell(2))), ValueMod.Plus(8)));        // the cause is under another ability
    a.Submit(new ResourceDraft(launch, B, R.Health, dmg, 100, default, inst.Node));
    Check("N9 from the launch, cause = the run: below yes, exactly no", w.Get(B, R.Health) == 1000 - (100 + 2 + 4));
    a.Submit(new ResourceDraft(launch, B, R.Health, dmg, 100));
    Check("N9 no cause: a filter that asks something about the cause does not match", w.Get(B, R.Health) == 1000 - 106 - (100 + 2));
    a.Submit(new ResourceDraft(inst.Node, B, R.Health, dmg, 100, default, inst.Node));
    Check("N9 from the run itself: exactly yes", w.Get(B, R.Health) == 1000 - 106 - 102 - (100 + 1 + 2 + 4));
}
{   // N10: IsSameOrDescendantOf: same inherited tags in two branches, so only the climb can tell them apart
    var a = New(out var w, out var clock);
    var inst = book.Start(1, Mage); var other = book.Start(2, Mage);
    var launch = a.Spawn(inst.Node, "launch");
    Check("N10 itself, its run, its ability, its entity, the root",
          launch.IsSameOrDescendantOf(launch) && launch.IsSameOrDescendantOf(inst.Node) && launch.IsSameOrDescendantOf(Spell(1))
          && launch.IsSameOrDescendantOf(a.EntitySource(Mage)) && launch.IsSameOrDescendantOf(a.Root));
    Check("N10 not another run / another ability (same inherited tags), not a child, not another entity",
          !launch.IsSameOrDescendantOf(other.Node) && !launch.IsSameOrDescendantOf(Spell(2)) && !inst.Node.IsSameOrDescendantOf(launch) && !launch.IsSameOrDescendantOf(a.EntitySource(B)));
    var plain = a.Spawn(a.Root, "plain"); var plain2 = a.Spawn(plain, "plain2");     // no tag at all: no short-circuit possible
    Check("N10 tag-less nodes fall back to the climb", plain2.IsSameOrDescendantOf(plain) && !plain.IsSameOrDescendantOf(plain2) && !plain2.IsSameOrDescendantOf(inst.Node));
}
{   // N11: lifecycle commands are requests: replaced, added to, observed
    var a = New(out var w, out var clock);
    // (a) a passive turns every hard kill of a spell run into a graceful termination
    a.Add(Rules.ReplaceSource(new(Command: SourceCommandKind.Kill, Node: new(ExactRequireAll: SourceTagSet.Of(SpellTags.Instance))),
                              Phase.Convert, r => r with { Command = SourceCommandKind.Terminate }));
    var inst = book.Start(1, Mage);
    var hold = new Hold();
    var projectile = a.Spawn(inst.Node, "projectile", default, hold);
    a.Kill(inst.Node);
    Check("N11 kill replaced by terminate: the run drains instead of vanishing", inst.Node.State == SourceState.PendingTermination && projectile.State == SourceState.PendingTermination && hold.Terminating);
    hold.Ticket.Complete();
    Check("N11 and leaves once its projectile is done", inst.Node.State == SourceState.Terminated);

    // (b) an addition on the termination of a run: refund 5 mana (from the entity node: the run itself is gone when it is processed)
    a.Add(Rules.AddOnSource(new(Command: SourceCommandKind.Terminate, Node: new(ExactRequireAll: SourceTagSet.Of(SpellTags.Instance))),
                            r => new Draft[] { new ResourceDraft(a.EntitySource(Mage), Mage, R.Mana, ResourceDirection.Add, 5) }));
    book.Start(1, Mage).Stop(StopReason.Completed);
    Check("N11 addition on a terminate request", w.Get(Mage, R.Mana) == 5);

    // (c) completions are requests too: the Done answer of an attachment is derived, the game's ticket is original
    var seen = new List<(Origin Origin, int Depth)>();
    a.Add(new Addition<SourceRequest>(r => r.Command == SourceCommandKind.CompleteTermination, r => { seen.Add((r.Origin, r.Depth)); return Array.Empty<Draft>(); }));
    var inst2 = book.Start(2, Mage); var hold2 = new Hold();
    a.Spawn(inst2.Node, "p", default, hold2);
    inst2.Stop(StopReason.Completed);
    hold2.Ticket.Complete();
    Check("N11 completions are observable: Done is derived (depth >= 1), the ticket of the game is original (depth 0)",
          seen.Any(x => x.Origin == Origin.Derived && x.Depth >= 1) && seen.Any(x => x.Origin == Origin.Original && x.Depth == 0));

    // (d) a reaction to a lifecycle fact is a derived request, like any reaction
    w.Set(B, R.Health, 100);
    a.Add(Rules.Mod(new(direction: ResourceDirection.Remove, Origin: OriginMask.Derived), ValueMod.Plus(1)));
    a.OnFact<SourceTerminated>(f => f.Node.Tags.Contains(SpellTags.Instance)
        ? new Draft[] { new ResourceDraft(a.EntitySource(Mage), B, R.Health, ResourceDirection.Remove, 10) } : None());
    book.Start(3, Mage).Stop(StopReason.Completed);
    Check("N11 reaction to SourceTerminated is derived (+1 from the derived-only modifier)", w.Get(B, R.Health) == 100 - 11);
}
{   // N12: find a node by what its attachment IS (capability interfaces of a layer the core does not know)
    var a = New(out var w, out var clock);
    w.Set(B, R.Health, 1000);
    var inst = book.Start(1, Mage);
    var launch = a.Spawn(inst.Node, "launch");
    var dmg = ResourceDirection.Remove;
    a.Add(Rules.Mod(new(direction: dmg, Source: SourceFilter.Having<IAbilityAttachment>(x => x.AbilityId == 1)), ValueMod.Plus(1)));    // the ability above is #1   yes
    a.Add(Rules.Mod(new(direction: dmg, Source: SourceFilter.Having<IAbilityAttachment>(x => x.AbilityId == 2)), ValueMod.Plus(2)));    // the ability above is #2   no
    a.Add(Rules.Mod(new(direction: dmg, Source: SourceFilter.HavingExact<IAbilityInstanceAttachment>()), ValueMod.Plus(4)));            // the node ITSELF is a run  launch: no
    a.Add(Rules.Mod(new(direction: dmg, Source: SourceFilter.Having<IAbilityInstanceAttachment>(r => !r.Stopped)), ValueMod.Plus(8)));  // a run above, still running yes
    a.Add(Rules.Mod(new(direction: dmg, Source: SourceFilter.Having<IAbilityAttachment>() with { ExactRequireAll = SourceTagSet.Of(SpellTags.Spell) }), ValueMod.Plus(16))); // fields combine: launch is no Spell itself: no
    a.Submit(new ResourceDraft(launch, B, R.Health, dmg, 100, default, inst.Node));
    Check("N12 from the launch: nearest ability / a running run above yes, itself a run no", w.Get(B, R.Health) == 1000 - (100 + 1 + 8));
    a.Submit(new ResourceDraft(inst.Node, B, R.Health, dmg, 100, default, inst.Node));
    Check("N12 from the run itself: exact capability matches too", w.Get(B, R.Health) == 1000 - 109 - (100 + 1 + 4 + 8));
}
{   // N13: the core announces facts of any type for the layers above it
    var a = New(out var w, out var clock);
    w.Set(B, R.Health, 100);
    int seen = 0;
    a.OnFact<string>(f => { seen++; return f == "boom" ? new Draft[] { new ResourceDraft(a.Root, B, R.Health, ResourceDirection.Remove, 10) } : None(); });
    a.Announce("hello");
    a.Announce("boom");
    Check("N13 a custom fact reaches its handlers, and its reactions are resolved even outside a run", seen == 2 && w.Get(B, R.Health) == 90);
}
{   // N8: the lifecycle processor still works on its own (no authority): it drains itself, hooks are queued, not nested
    var p = new SourcesProcessor();
    var parent = new Source(1, "parent"); var child = new Source(2, "child");
    p.Insert(p.Root, parent); p.Insert(parent, child);
    p.Terminate(parent);
    Check("N8 standalone: inserted, terminated child first, nothing left", parent.State == SourceState.Terminated && child.State == SourceState.Terminated && p.Root.Children.Count == 0);
}

// =====================  tags: chunks, hierarchy, domains  =====================

bool Throws(Action act) { try { act(); return false; } catch (Exception) { return true; } }

{   // G1: more than 64 tags; sets built before the registry grew still compare correctly
    var early = Tag<TG1>.Register("t0");
    var setEarly = TagSet<TG1>.Of(early);
    for (var i = 1; i < 200; i++) Tag<TG1>.Register("t" + i);
    var late = Tag<TG1>.Register("t150");
    var both = TagSet<TG1>.Of(early, late);
    Check("G1 200 tags, indices past 64 work", TagRegistry<TG1>.Count == 200 && late.Index == 150 && both.Contains(late));
    Check("G1 a shorter set is contained in a longer one, not the reverse", both.ContainsAll(setEarly) && !setEarly.ContainsAll(both));
    Check("G1 Intersects across chunks", both.Intersects(TagSet<TG1>.Of(late)) && !setEarly.Intersects(TagSet<TG1>.Of(late)));
}
{   // G2: hierarchy: a set holding Spell.Fire also holds Spell
    var fire = Tag<TG2>.Register("Spell.Fire"); var ice = Tag<TG2>.Register("Spell.Ice"); var dmg = Tag<TG2>.Register("Damage");
    var spell = Tag<TG2>.Register("Spell");        // registered implicitly by Spell.Fire: same tag
    var s = TagSet<TG2>.Of(fire);
    Check("G2 parents are registered implicitly, once", TagRegistry<TG2>.Count == 4 && spell.Index == 0 && fire.Index == 1);
    Check("G2 Spell.Fire matches Spell and itself, not Spell.Ice", s.Contains(spell) && s.Contains(fire) && !s.Contains(ice));
    Check("G2 ContainsAll / Intersects follow the hierarchy", s.ContainsAll(TagSet<TG2>.Of(spell)) && !s.ContainsAll(TagSet<TG2>.Of(ice)) && s.Intersects(TagSet<TG2>.Of(spell)));
    Check("G2 parent / descendant queries", fire.Parent == spell && spell.Parent is null && fire.IsSameOrDescendantOf(spell) && !spell.IsSameOrDescendantOf(fire));
    Check("G2 ToString shows the most specific tags", TagSet<TG2>.Of(fire, dmg).ToString() == "{Spell.Fire, Damage}");
}
{   // G3: removal removes the tag and its descendants, ancestors stay
    var fire = Tag<TG3>.Register("Spell.Fire"); var ice = Tag<TG3>.Register("Spell.Ice"); var dmg = Tag<TG3>.Register("Damage");
    var spell = Tag<TG3>.Register("Spell");
    var s = TagSet<TG3>.Of(fire, ice, dmg);
    var noFire = s.Without(fire);
    Check("G3 removing Spell.Fire keeps Spell.Ice and therefore Spell", !noFire.Contains(fire) && noFire.Contains(ice) && noFire.Contains(spell) && noFire.Contains(dmg));
    var noSpell = s.Without(spell);
    Check("G3 removing Spell removes every Spell.*", !noSpell.Contains(spell) && !noSpell.Contains(fire) && !noSpell.Contains(ice) && noSpell.Contains(dmg));
}
{   // G5: RegisterAll is order independent; the fingerprint tells two layouts apart
    TagRegistry<TG5a>.RegisterAll(new[] { "Spell.Fire", "Damage", "Spell.Ice" });
    TagRegistry<TG5b>.RegisterAll(new[] { "Damage", "Spell.Ice", "Spell.Fire" });
    Check("G5 same names, any order: same layout and fingerprint",
          TagRegistry<TG5a>.Fingerprint == TagRegistry<TG5b>.Fingerprint
          && Tag<TG5a>.Register("Spell.Ice").Index == Tag<TG5b>.Register("Spell.Ice").Index);
    Tag<TG5b>.Register("Other");
    Check("G5 a different set of names gives a different fingerprint", TagRegistry<TG5a>.Fingerprint != TagRegistry<TG5b>.Fingerprint);
    Check("G5 RegisterAll refuses a registry that already has tags", Throws(() => TagRegistry<TG5a>.RegisterAll(new[] { "X" })));
}
{   // G6: freeze
    var a = Tag<TG6>.Register("Known");
    TagRegistry<TG6>.Freeze();
    Check("G6 frozen: a known name still resolves, an unknown one is rejected",
          Tag<TG6>.Register("Known") == a && Throws(() => Tag<TG6>.Register("Unknown")) && TagRegistry<TG6>.IsFrozen);
}
{   // G7: declared roots catch typos
    TagRegistry<TG7>.DeclareRoots("Spell", "Damage");
    Tag<TG7>.Register("Spell.Fire");
    Check("G7 an undeclared root is rejected (Spel.Fire)", Throws(() => Tag<TG7>.Register("Spel.Fire")) && Throws(() => Tag<TG7>.Register("Spell..Fire")));
}
{   // G8: canonical form: equality does not depend on how a set was built
    var a = Tag<TG8>.Register("a"); var b = Tag<TG8>.Register("b");
    for (var i = 0; i < 100; i++) Tag<TG8>.Register("pad" + i);
    var far = Tag<TG8>.Register("pad99");
    Check("G8 removing everything gives the empty set", TagSet<TG8>.Of(a, far).Without(a).Without(far) == default && TagSet<TG8>.Of(a).Without(a).IsEmpty);
    Check("G8 construction order does not matter", TagSet<TG8>.Of(a, b) == TagSet<TG8>.Of(b, a)
          && TagSet<TG8>.Of(a, b).GetHashCode() == TagSet<TG8>.Of(b, a).GetHashCode());
    Check("G8 trailing empty chunks do not matter", TagSet<TG8>.Of(a, far).Without(far) == TagSet<TG8>.Of(a));
    Check("G8 Union", TagSet<TG8>.Of(a).Union(TagSet<TG8>.Of(far)) == TagSet<TG8>.Of(a, far));
}
{   // G9: domains are independent registries
    Tag<TG9a>.Register("x");
    Tag<TG9b>.Register("y"); Tag<TG9b>.Register("z");
    Check("G9 two domains, two registries (compile-time separation too: Tag<TG9a> and Tag<TG9b> do not mix)",
          TagRegistry<TG9a>.Count == 1 && TagRegistry<TG9b>.Count == 2 && Tag<TG9a>.Register("x").Index == 0);
}

Console.WriteLine($"\n{passed} passed, {failed} failed");
return failed == 0 ? 0 : 1;
