using Godot;
using WowGd.Src.Entities;
using WowGd.Src.Input.Hands;
using WowGd.Src.Tools;

namespace WowGd.Src.Physics.Movement;

[GlobalClass]
public partial class MoveModeInput : Node, IFirstHandInputMode
{
    private IEntity _entity = null!;
    private MoveMode _mode = null!;

    public bool CanStart() => true;
    public bool CanStop() => true;

    public override async void _Ready()
    {
        if (this.TryGetComposed(out MoveMode? mode))
            _mode = mode;

        if (!this.TryGetComposedRecursive(out IEntity? entity))
            return;

        _entity = entity;

        await _entity.Initialization;
        HandsInputManager.TryPushFirstHandMode(this);
    }

    public void Start()
    {
        _entity.WishDir.Enable();
        SetProcessUnhandledKeyInput(true);
    }

    public void Stop()
    {
        _entity.WishDir.Disable();
        SetProcessUnhandledKeyInput(false);
    }

    public override void _UnhandledKeyInput(InputEvent @event)
    {
        if (@event.IsMovementAbilityPressed())
            _mode.StartMovementAbility();
        else if (@event.IsLockingPressed())
            _mode.Lock();
        else if (@event.IsDodgingPressed())
            _mode.Dodge();
    }
}