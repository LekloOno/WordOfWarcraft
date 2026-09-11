using Godot;
using WowGd.Src.Input.Hands;
using WowGd.Src.Render.WorldRenderer.Entities;

namespace WowGd.Src.Render.Ui.Input.Impls.Worder;

public partial class WorderDisplay : Node, IHandInputUi
{
    [Export] public Control ControlNode { get; private set; } = null!;
    [Export] private Control _unactiveLayer = null!;
    [Export] private ColorRect _caretColor = null!;
    [Export] private Color _caretActiveColor;
    [Export] private Color _caretUnactiveColor;

    public IHandsInputUiContext Context => HandsInputUiContext.Instance;
    public HandInputDocking Docking => HandInputDocking.Main;
    public IListenableHandInputMode InputMode => Driver;


    private void ReadyHandInput()
    {
        this.BindToContext();
        SetUnactive();
        HideHand();
        PlayerInputUiSyncer.Register(this);
    }

    public void ShowHand()
    {
        ControlNode.Show();
        UpdateCursor(_lastCursorIdx);
    }

    public void HideHand()
    {
        ControlNode.Hide();
    }

    public void SetActive()
    {
        _unactiveLayer.Hide();
        _caretColor.Color = _caretActiveColor;
    }

    public void SetUnactive()
    {
        _unactiveLayer.Show();
        _caretColor.Color = _caretUnactiveColor;
    }
}