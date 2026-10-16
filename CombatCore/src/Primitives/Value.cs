namespace CombatCore.Primitives;

/// <summary>
/// A modification to feed into a request's value. Stored as "multiplier - 1" so that default(ValueMod) is the identity.
/// </summary>
public readonly struct ValueMod
{
    readonly double _multMinusOne;
    public long Flat { get; }
    public double Mult => 1 + _multMinusOne;

    ValueMod(long flat, double mult) { Flat = flat; _multMinusOne = mult - 1; }

    public static ValueMod Plus(long flat) => new(flat, 1);
    public static ValueMod Times(double mult) => new(0, mult);
}

public readonly record struct Contribution(uint By, string Kind, double Amount);

/// <summary>
/// Request-local value: base + flat sums, then a multiplier. One-shot, no removal handles (unlike stat buckets).
/// Keeps who contributed what, so the fact can say "spell 20, passive Z +5".
/// </summary>
public sealed class BucketedValue(long baseValue)
{
    readonly List<Contribution> _contributions = new();
    long _flat;
    double _mult = 1;

    public long Base { get; } = baseValue;
    public IReadOnlyList<Contribution> Contributions => _contributions;

    public void Apply(uint by, ValueMod mod)
    {
        if (mod.Flat != 0) { _flat += mod.Flat; _contributions.Add(new(by, "flat", mod.Flat)); }
        if (mod.Mult != 1) { _mult *= mod.Mult; _contributions.Add(new(by, "mult", mod.Mult)); }
    }

    public long Resolve() => Math.Max(0, (long)Math.Round((Base + _flat) * _mult));

    public BucketedValue Clone()
    {
        var c = new BucketedValue(Base) { _flat = _flat, _mult = _mult };
        c._contributions.AddRange(_contributions);
        return c;
    }
}
