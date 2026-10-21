using CombatCore.Primitives;

namespace CombatCore.Tests;


// In-memory implementations of the core's ports: this is what lets the core run (and be tested) without Godot.
// The Godot project provides the real ones (adapters over IEntity, IResourceManager, IStatistics, the physics tick).

public sealed class ManualClock : IGameClock { public long Tick { get; set; } }

public static class R { public static readonly ResourceId Health = new(0), Mana = new(1), Stamina = new(2); }

public sealed class FakeEntity(EntityId id) : ICombatEntity, IResourcePort, IStatPort
{
    readonly Dictionary<ResourceId, (long Cur, long Max)> _res = new();
    readonly List<Mod> _mods = new();
    public int Notifications { get; private set; }

    public EntityId Id { get; } = id;
    public IResourcePort Resources => this;
    public IStatPort Stats => this;
    public ResourceId LethalResource { get; set; } = R.Health;

    public void Set(ResourceId r, long value, long max = long.MaxValue) => _res[r] = (value, max);
    public long Get(ResourceId r) => _res.TryGetValue(r, out var v) ? v.Cur : 0;

    // ---- IResourcePort: clamping lives here, like in the real resources ----
    public bool Has(ResourceId id) => _res.ContainsKey(id);
    public ResourceChange Apply(ResourceId id, ResourceDirection dir, long amount)
    {
        var (cur, max) = _res[id];
        long next = dir == ResourceDirection.Remove ? Math.Max(0, cur - amount) : Math.Min(max, cur + amount);
        _res[id] = (next, max);
        return new ResourceChange(cur, next, Killed: id == LethalResource && cur > 0 && next == 0);
    }

    // ---- IStatPort: same formula as BucketedIntStat, with weight scaling the delta of a multiplier ----
    sealed class Mod(int flat, float mult, float weight)
    {
        public int Flat = flat; public float Mult = mult; public float Weight = weight;
        public int FlatValue => (int)Math.Floor(Flat * Weight);
        public float MultValue => Math.Max(0f, 1f + (Mult - 1f) * Weight);
        public StatId Stat;
    }

    sealed class Handle(FakeEntity e, Mod m) : IEffectHandle
    {
        bool _disposed;
        public void Reweight(float weight) { if (_disposed) return; m.Weight = weight; e.Notifications++; }
        public void Dispose() { if (_disposed) return; _disposed = true; e._mods.Remove(m); e.Notifications++; }
    }

    public IEffectHandle Apply(StatContribution c, float weight)
    {
        var m = new Mod(c.Flat, c.Mult, weight) { Stat = c.Stat };
        _mods.Add(m); Notifications++;
        return new Handle(this, m);
    }

    public int Stat(StatId s, int baseValue)
    {
        float mult = 1f; int flat = 0;
        foreach (var m in _mods.Where(m => m.Stat == s)) { mult *= m.MultValue; flat += m.FlatValue; }
        return (int)Math.Floor(baseValue * mult) + flat;
    }
}

public sealed class FakeWorld : IEntityDirectory
{
    readonly Dictionary<EntityId, FakeEntity> _entities = new();
    public FakeEntity Add(EntityId id) => _entities[id] = new FakeEntity(id);
    public void Remove(EntityId id) => _entities.Remove(id);
    public ICombatEntity? Find(EntityId id) => _entities.GetValueOrDefault(id);
    public FakeEntity this[EntityId id] => _entities[id];

    public void Set(EntityId e, ResourceId r, long value, long max = long.MaxValue) => _entities[e].Set(r, value, max);
    public long Get(EntityId e, ResourceId r) => _entities[e].Get(r);
    public int Stat(EntityId e, StatId s, int baseValue) => _entities[e].Stat(s, baseValue);
    public int Notifications(EntityId e) => _entities[e].Notifications;
}
/// <summary>Typed payload used by the typed-status tests (stands for the mage's BreachStatus).</summary>
public sealed class FakeBreaches(EntityId owner) : IDisposable
{
    public EntityId Owner { get; } = owner;
    public int Generation { get; set; } = 1;     // bumped by the reapply hook: "Brèches re-cast re-rolls the positions"
    public bool Disposed { get; private set; }
    public void Dispose() => Disposed = true;
}

public sealed class DisposeFlag : IDisposable { public bool Disposed { get; private set; } public void Dispose() => Disposed = true; }
