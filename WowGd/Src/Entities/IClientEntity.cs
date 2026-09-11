using WowGd.Src.Combat.Abilities.Actuation.Drivers;
using WowGd.Src.Input;

namespace WowGd.Src.Entities;

/// <summary>
/// Extension of entity to retrieve client specific nodes, typically input related ones.
/// </summary>
public interface IClientEntity : IEntity
{
    PlayerDactyloDriver         DactyloDriver       { get; }
    SelectionCombatModeInput    SelectionModeInput  { get; }
}