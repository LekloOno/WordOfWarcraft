using System;

namespace WowGd.Src.Combat.Health;

public class EnvironmentHealth : IEntityHealth
{
    public readonly static EnvironmentHealth Instance = new();

    public int HitPoints => 0;
    public int Current => 0;

    public event Action<int>?   Damaged;
    public event Action<int>?   Healed;
    public event Action?        Died;
    public event Action<int>?   Resurrected;

    public bool Damage(int hp) => false;
    public bool Dead() => false;
    public bool Heal(int hp) => false;
    public void Resurrect(int? hp) {}
}