using WowGd.Src.Combat.Abilities.Targeting.Payload;

namespace WowGd.Src.Combat.Abilities.Data;

/// <summary>
/// An effect, mutation of entities.
/// Damage, heal, resurrect, buffs, debuffs, and any other specific status effect.
/// 
/// <para>
/// Effects are stateless, so there's no "IEffectData".
/// We could separate the apply logic from the data, but I don't think it's worth.
/// Effects can use states, but only through the targets payload.
/// </para>
/// 
/// <para>
/// For example, for a combo system, the effect would not store the attacks buffer on itself.
/// Instead, it should add a specific status on the entity itself.
/// That is why the effects themselves are stateless.
/// </para>
/// </summary>
public interface IEffect
{
    string Id { get; }
    bool Apply(TargetsPayload targetsPayload);
}