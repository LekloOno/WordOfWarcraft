using Godot;
using WowGd.Src.Combat.Abilities.Targeting.Player;
using WowGd.Src.Input.Hands;
using WowGd.Src.Input.Targeting.Direct;
using WowGd.Src.Render.WorldRenderer.Entities;

namespace WowGd.Src.Render.Ui.Input.Impls;

[GlobalClass]
public partial class DirectTargetUi : Node, IHandInputUi
{
    [Export] private PlayerTargetIntentDriver _targetDriver = null!;
    [Export] public PlayerTargetIntentDriver TargetDriver
    {
        get => _targetDriver;
        set
        {
            if (value == _targetDriver)
                return;

            if (_targetDriver != null)
                this.UnbindToContext();

            _targetDriver = value;
            if (_targetDriver != null)
                this.BindToContext();
        }
    }

    public IHandsInputUiContext Context => HandsInputUiContext.Instance;
    public HandInputDocking Docking => HandInputDocking.None;
    public IListenableHandInputMode InputMode => TargetDriver.DirectDriver;

    public override void _Ready()
    {
        PlayerInputUiSyncer.Register(this);
    }

    public void ShowHand()
    {
        DirectTargetEntitiesManager.Show();
    }

    public void HideHand()
    {
        DirectTargetEntitiesManager.Hide();
    }

    public void SetActive() { }
    public void SetUnactive() { }
}