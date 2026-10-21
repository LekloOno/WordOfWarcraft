using CombatCore;
using CombatCore.Primitives;
using CombatCore.Tests;

int failed = 0, passed = 0;
void Check(string name, bool ok) { if (ok) passed++; else failed++; Console.WriteLine($"{(ok ? "PASS" : "FAIL")}  {name}"); }
void Show(Authority a, string title) { Console.WriteLine($"\n--- {title}"); foreach (var l in a.Log) Console.WriteLine("  " + l); Console.WriteLine(); }

var Mage = new EntityId(1); var B = new EntityId(2); var C = new EntityId(3); var X = new EntityId(4);

// A fresh authority over a fake world. Every entity starts with empty health/mana/stamina pools.
Authority New(out FakeWorld w, out ManualClock clock, int maxDepth = 8)
{
    var world = new FakeWorld(); clock = new ManualClock();
    foreach (var e in new[] { Mage, B, C, X })
    { world.Add(e); world.Set(e, R.Health, 0); world.Set(e, R.Mana, 0); world.Set(e, R.Stamina, 0); }
    w = world;
    return new Authority(clock, world) { MaxDepth = maxDepth };
}
void At(Authority a, ManualClock c, long tick) { c.Tick = tick; a.Update(); }
Source Spell(uint id) => new(SourceKind.Ability, id, Mage);
Source Status(uint id) => new(SourceKind.Status, id, Mage);
Source Sys(EntityId owner) => new(SourceKind.System, 0, owner);
IEnumerable<Draft> None() => Array.Empty<Draft>();

// =====================  part 1: pipeline (unchanged behavior)  =====================

{   // drain becomes damage, then +5
    var a = New(out var w, out var clock);
    w.Set(B, R.Health, 100); w.Set(B, R.Mana, 100);
    a.Add(Rules.Replace(new(direction: ResourceDirection.Remove), Phase.Convert, r => r with { Direction = ResourceDirection.Remove, Resource = R.Health }));
    a.Add(Rules.Mod(new(direction: ResourceDirection.Remove), ValueMod.Plus(5)));
    a.Submit(new RequestDraft(Spell(1), B, R.Mana, ResourceDirection.Remove, 20));
    Check("P1 drain -> damage +5: health 75, mana untouched", w.Get(B, R.Health) == 75 && w.Get(B, R.Mana) == 100);
}
{   // competing replacements
    var a = New(out var w, out var clock);
    w.Set(B, R.Health, 100); w.Set(B, R.Stamina, 100); w.Set(B, R.Mana, 100);
    var drain = Tag.Register("test.drain"); var dmg = Tag.Register("test.damage");
    // both match "drain"; the first converts it (rewrites the tags), so the second no longer matches
    a.Add(Rules.Replace(new(RequireAll: TagSet.Of(drain)), Phase.Convert, r => r with { Resource = R.Health, Tags = TagSet.Of(dmg) }));
    a.Add(Rules.Replace(new(RequireAll: TagSet.Of(drain)), Phase.Convert, r => r with { Resource = R.Stamina, Tags = TagSet.Of(dmg) }));
    a.Submit(new RequestDraft(Spell(1), B, R.Mana, ResourceDirection.Remove, 20, TagSet.Of(drain)));
    Check("P2 first replacement wins", w.Get(B, R.Health) == 80 && w.Get(B, R.Stamina) == 100);
}
{   // lineage rule
    var a = New(out var w, out var clock);
    w.Set(B, R.Health, 1000); w.Set(B, R.Mana, 1000);
    var drain = Tag.Register("test.drain"); var dmg = Tag.Register("test.damage");
    a.Add(Rules.AddOn(new(RequireAll: TagSet.Of(dmg)),   r => new[] { new RequestDraft(Status(1), B, R.Mana, ResourceDirection.Remove, 5, TagSet.Of(drain)) }));
    a.Add(Rules.AddOn(new(RequireAll: TagSet.Of(drain)), r => new[] { new RequestDraft(Status(2), B, R.Health, ResourceDirection.Remove, 5, TagSet.Of(dmg)) }));
    a.Submit(new RequestDraft(Spell(1), B, R.Health, ResourceDirection.Remove, 10, TagSet.Of(dmg)));
    Check("P3 lineage: terminates after one round, nothing dropped",
          w.Get(B, R.Health) == 985 && w.Get(B, R.Mana) == 995 && !a.Log.Any(l => l.StartsWith("DROP")));
}
{   // depth fuse
    var a = New(out var w, out var clock, maxDepth: 4);
    w.Set(B, R.Health, 1000);
    a.OnFact<ResourceFact>(f => new[] { new RequestDraft(Status(9), B, R.Health, ResourceDirection.Remove, 1) });
    a.Submit(new RequestDraft(Spell(1), B, R.Health, ResourceDirection.Remove, 1));
    Check("P4 depth fuse", w.Get(B, R.Health) == 995 && a.Log.Any(l => l.StartsWith("DROP")));
}
{   // direct vs derived discrimination
    var a = New(out var w, out var clock);
    w.Set(B, R.Health, 100); w.Set(C, R.Health, 100);
    a.Add(Rules.Mod(new(direction: ResourceDirection.Remove, Kind: SourceKind.Ability, Origin: OriginMask.Original), ValueMod.Plus(5)));
    a.Add(Rules.Mod(new(direction: ResourceDirection.Remove), ValueMod.Plus(1)));
    a.OnFact<ResourceFact>(f => f.Source.Kind == SourceKind.Ability && f.Direction == ResourceDirection.Remove
        ? new[] { new RequestDraft(Status(7), C, R.Health, ResourceDirection.Remove, 10) } : None());
    a.Submit(new RequestDraft(Spell(1), B, R.Health, ResourceDirection.Remove, 20));
    Check("P5 direct-only modifier skips derived damage", w.Get(B, R.Health) == 74 && w.Get(C, R.Health) == 89);
}
{   // rollback: refund on kill reads the cost fact
    foreach (var free in new[] { false, true })
    {
        var costTag = Tag.Register("cost");
        var tagSet = TagSet.Of(costTag);
        var a = New(out var w, out var clock);
        var instanceAbility = new Dictionary<InstanceId, uint> { [new(41)] = 1 };
        var optedIn = new HashSet<uint> { 1 };
        var spent = new Dictionary<InstanceId, long>();
        w.Set(Mage, R.Mana, 100); w.Set(X, R.Health, 10);
        if (free) a.Add(Rules.Mod(new(direction: ResourceDirection.Remove, SourceId: 1), ValueMod.Times(0)));
        a.OnFact<ResourceFact>((Func<ResourceFact, IEnumerable<Draft>>)(f => { if (f.Direction == ResourceDirection.Remove && f.Ctx.Cause is { } c && f.Tags.ContainsAll(tagSet)) spent[c] = spent.GetValueOrDefault(c) + f.Applied; return None(); }));
        a.OnFact<DeathFact>(d => d.Ctx.Cause is { } c && instanceAbility.TryGetValue(c, out var ab) && optedIn.Contains(ab) && spent.GetValueOrDefault(c) > 0
            ? new Draft[] { new RequestDraft(Status(99), Mage, R.Mana, ResourceDirection.Add, spent[c], default, c) } : None());
        var inst = new InstanceId(41);
        a.Submit(new RequestDraft(Spell(1), Mage, R.Mana, ResourceDirection.Remove, 30, tagSet, inst));
        a.Submit(new RequestDraft(Status(5), X, R.Health, ResourceDirection.Remove, 40, default, inst));
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
            f.Direction == ResourceDirection.Remove && f.Source.Id == 1 && f.Source.Owner == inst.Owner
                ? new Draft[] { new StatusDraft(Status(200), inst.Owner, StatusOp.Adjust, free, Level: 1, LevelOp: MergeOp.Subtract) }
                : None()) });
    w.Set(Mage, R.Mana, 100);
    Check("S4 cost stat 30 before", w.Stat(Mage, Cost, 30) == 30);
    a.Submit(new StatusDraft(Spell(2), Mage, StatusOp.Apply, free, Level: 1));
    int effective = w.Stat(Mage, Cost, 30);
    Check("S4 UI/precondition read effective cost 0", effective == 0);
    a.Submit(new RequestDraft(Spell(1), Mage, R.Mana, ResourceDirection.Remove, effective));     // the cast spends the effective cost
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
            new(direction: ResourceDirection.Remove, SourceOwner: inst.Owner), _ => ValueMod.Times(1 - 0.1 * inst.Weight))) });
    a.Submit(new StatusDraft(Status(9), B, StatusOp.Apply, weakened, Level: 1));
    a.Submit(new RequestDraft(new Source(SourceKind.Ability, 1, B), C, R.Health, ResourceDirection.Remove, 100));
    w.Set(C, R.Health, 1000);
    a.Submit(new RequestDraft(new Source(SourceKind.Ability, 1, B), C, R.Health, ResourceDirection.Remove, 100));
    long afterL1 = w.Get(C, R.Health);
    a.Submit(new StatusDraft(Status(9), B, StatusOp.Apply, weakened, Level: 1));        // level 2
    w.Set(C, R.Health, 1000);
    a.Submit(new RequestDraft(new Source(SourceKind.Ability, 1, B), C, R.Health, ResourceDirection.Remove, 100));
    Check("S5 level 1: 100 -> 90, level 2: 100 -> 80", afterL1 == 910 && w.Get(C, R.Health) == 920);
}
{   // S6: cleanse by category; passives are never swept by a catch-all
    var a = New(out var w, out var clock);
    var Slowed = Tag.Register("Slowed");
    var buff = Might(); 
    var debuff = new StatusDefinition("Slow", StatusCategory.Debuff, Grants: TagSet.Of(Slowed));
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
    a.Submit(new RequestDraft(Spell(1), B, R.Health, ResourceDirection.Remove, 10));
    At(a, clock, 10);
    a.Submit(new RequestDraft(Spell(1), B, R.Health, ResourceDirection.Remove, 10));
    Check("S8 +5 while active (15), gone after expiry (10)", w.Get(B, R.Health) == 75);
}

// =====================  part 3: spell lifecycle  =====================

{   // L1: the spell asks for an instance, emits through it, stops once with a reason
    var a = New(out var w, out var clock);
    w.Set(B, R.Health, 100); w.Set(Mage, R.Mana, 100);
    var started = new List<InstanceStarted>(); var stopped = new List<InstanceStopped>(); var facts = new List<ResourceFact>();
    a.OnFact<InstanceStarted>(f => { started.Add(f); return None(); });
    a.OnFact<InstanceStopped>(f => { stopped.Add(f); return None(); });
    a.OnFact<ResourceFact>(f => { facts.Add(f); return None(); });

    var inst = a.StartInstance(abilityId: 7, caster: Mage);
    inst.Request(Mage, R.Mana, ResourceDirection.Remove, 30);
    inst.Request(B, R.Health, ResourceDirection.Remove, 20);
    inst.Stop(StopReason.Completed);
    inst.Stop(StopReason.Cancelled);                      // ignored: a flow ends once
    Show(a, "L1: spell lifecycle");
    Check("L1 started and stopped once, with the first reason", started.Count == 1 && stopped.Count == 1 && stopped[0].Reason == StopReason.Completed);
    Check("L1 every fact carries the instance as cause and the ability as source",
          facts.Count == 2 && facts.All(f => f.Ctx.Cause == inst.Id && f.Source == new Source(SourceKind.Ability, 7, Mage)));
    Check("L1 effects applied", w.Get(Mage, R.Mana) == 70 && w.Get(B, R.Health) == 80);
}
{   // L2: a stopped instance may still emit (a projectile landing after the flow ended)
    var a = New(out var w, out var clock);
    w.Set(B, R.Health, 100);
    var inst = a.StartInstance(7, Mage);
    inst.Stop(StopReason.Completed);
    inst.Request(B, R.Health, ResourceDirection.Remove, 10);
    Check("L2 consequences outlive the flow", w.Get(B, R.Health) == 90);
}
{   // L3: a reaction to a lifecycle fact (the hook you want to bind things on)
    var a = New(out var w, out var clock);
    w.Set(Mage, R.Mana, 50);
    a.OnFact<InstanceStopped>(f => f.Reason == StopReason.Interrupted
        ? new Draft[] { new RequestDraft(Status(1), f.Caster, R.Mana, ResourceDirection.Add, 10) } : None());
    a.StartInstance(7, Mage).Stop(StopReason.Completed);
    Check("L3 no reaction on Completed", w.Get(Mage, R.Mana) == 50);
    a.StartInstance(7, Mage).Stop(StopReason.Interrupted);
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
    a.Submit(new RequestDraft(Spell(1), B, R.Health, ResourceDirection.Remove, 10));
    Check("E1 request to a removed entity is dropped", a.Log.Any(l => l.Contains("target gone")));
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

Console.WriteLine($"\n{passed} passed, {failed} failed");
return failed == 0 ? 0 : 1;
