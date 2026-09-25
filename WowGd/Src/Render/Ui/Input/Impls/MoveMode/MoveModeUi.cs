using Godot;
using WowGd.Src.Combat.Abilities;
using WowGd.Src.Input.Hands;
using WowGd.Src.Physics.Movement;
using WowGd.Src.Render.WorldRenderer.Entities;

namespace WowGd.Src.Render.Ui.Input.Impls.MoveMode;

[GlobalClass]
public partial class MoveModeUi : Node, IHandInputUi
{
    [Export] private Control _container = null!;
    [Export] private AbilityCellUi _movementUi = null!;
    [Export] private WasdCellUi _up = null!;
    [Export] private WasdCellUi _down = null!;
    [Export] private WasdCellUi _left = null!;
    [Export] private WasdCellUi _right = null!;

    private MoveModeInput _moveModeInput = null!;
    [Export] public MoveModeInput MoveModeInput
    {
        get => _moveModeInput;
        set
        {
            if (value == _moveModeInput)
                return;

            if (_moveModeInput != null)
                this.UnbindToContext();

            _moveModeInput = value;
            if (_moveModeInput == null)
                return;

            this.BindToContext();

            if (_moveModeInput.Mode.MovementAbility is IAbility listenable)
            {
                _movementUi.Ability = listenable;
                _movementUi.SetInput(MoveModeInputExt.MovementAbility);
            }
        }
    }

    public IHandsInputUiContext Context => HandsInputUiContext.Instance;
    public HandInputDocking Docking => HandInputDocking.Main;
    public IListenableHandInputMode InputMode => _moveModeInput;

    public override void _Ready()
    {
        PlayerInputUiSyncer.Register(this);
    }

    public void ShowHand()
    {
        _container.Show();
    }

    public void HideHand()
    {
        _container.Hide();
    }

    public void SetActive()
    {
        _movementUi.SetActive();
        _up.SetActive();
        _down.SetActive();
        _left.SetActive();
        _right.SetActive();
    }

    public void SetUnactive()
    {
        _movementUi.SetUnactive();
        _up.SetUnactive();
        _down.SetUnactive();
        _left.SetUnactive();
        _right.SetUnactive();
    }
}