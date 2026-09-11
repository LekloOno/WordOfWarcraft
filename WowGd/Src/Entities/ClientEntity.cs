using Godot;
using WowGd.Src.Combat.Abilities.Actuation.Drivers;
using WowGd.Src.Input;
using WowGd.Src.Physics.Movement;

namespace WowGd.Src.Entities;

[GlobalClass]
public partial class ClientEntity : Entity, IClientEntity
{
    [Export] public PlayerDactyloDriver DactyloDriver { get; private set; } = null!;
    [Export] public SelectionCombatModeInput SelectionModeInput { get; private set; } = null!;
    [Export] public MoveModeInput MoveModeInput { get; private set; } = null!;
}