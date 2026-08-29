namespace WowGd.Src.Combat.Abilities.Exp.Data;

/// <summary>
/// An effect, mutation of entities.
/// Damage, heal, resurrect, buffs, debuffs, and any other specific status effect.
/// </summary>
public interface IEffectData
{
    string Id { get; }
}