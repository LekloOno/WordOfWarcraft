using WowGd.Src.Entities;

namespace WowGd.Src.Combat.Abilities.Data;

/// <summary>
/// A precondition, based on the state of the caster entity.
/// </summary>
public interface ICasterRule
{
    string Id { get; }
    bool Check(IEntity caster);
}