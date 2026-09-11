using System;
using Godot;
using WowGd.Src.Combat.Modes;
using WowGd.Src.Input.Hands;

namespace WowGd.Src.Input;

[GlobalClass]
public partial class SelectionCombatModeInput : Node, ISecondHandInputMode, IListenableHandInputMode
{
    [Export] public SelectionCombatMode Mode { get; private set; } = null!;

    private bool _active;

    public event Action? InputStarted;
    public event Action? InputStopped;
    public event Action? InputPushed;
    public event Action? InputRemoved;


    public override void _Ready()
    {
        if (HandsInputManager.TryPushSecondHandMode(this))
            InputPushed?.Invoke();
    }

    public override void _UnhandledKeyInput(InputEvent @event)
    {
        if (Mode.HasActive())
        {
            if (@event.IsActionPressed("ability_cancel"))
                Mode.Unselect(0);  // ad-hoc, this is wip, this would suppose no one else is interracting with the selection
            return;
        }

        if (!_active)
            return;

        if (@event.TryGetAbilityIndex(out int index))
            Mode.Select(index);
    }

    public bool CanStart() => true;
    public bool CanStop() => true;

    public void Start()
    {
        _active = true;
        InputStarted?.Invoke();
    }

    public void Stop()
    {
        _active = false;
        InputStopped?.Invoke();
    }
}