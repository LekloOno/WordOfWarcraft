using CombatCore;
using Godot;
using Godot.Collections;
using WowGd.Src.Combat;
using WowGd.Src.Combat.Abilities.Actuation.Drivers;
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
    [Export] private Array<StatusData> _testStatuses = [];

    public override async void _Ready()
    {
        base._Ready();

        await CombatCoreNode.Instance.Initialization;

        foreach (StatusData status in _testStatuses)
            status.Submit(this, this, StatusOp.Apply, duration: 600);
    }
}