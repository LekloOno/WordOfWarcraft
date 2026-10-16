using Godot;
using WowGd.Src.Combat;
using WowGd.Src.Combat.Abilities.Actuation.Drivers;
using WowGd.Src.Combat.Resources;
using WowGd.Src.Combat.Resources.FocusRes;
using WowGd.Src.Combat.Statuses;
using WowGd.Src.Input;
using WowGd.Src.Physics.Movement;

namespace WowGd.Src.Entities;

[GlobalClass]
public partial class ClientEntity : Entity, IClientEntity
{
    [Export] public PlayerDactyloDriver DactyloDriver { get; private set; } = null!;
    [Export] public SelectionCombatModeInput SelectionModeInput { get; private set; } = null!;
    [Export] public MoveModeInput MoveModeInput { get; private set; } = null!;
    [Export] private StatusData? _testStatus;

    public override async void _Ready()
    {
        base._Ready();

        await CombatCoreNode.Instance.Initialization;

        _testStatus?.Submit(this, this, duration: 300);
    }
}