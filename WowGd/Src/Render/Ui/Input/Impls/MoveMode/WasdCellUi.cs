using Godot;

namespace WowGd.Src.Render.Ui.Input.Impls.MoveMode;

[GlobalClass]
public partial class WasdCellUi : Control
{
    [Export] private WasdEnum   _direction;
    [Export] private Label      _inputLabel = null!;
    [Export] private ColorRect  _unactiveLayer  = null!;
    [Export] private StyleBoxFlat _pressedStyle = null!;
    [Export] private int MaxBorderRadius = 4;
    [Export] private float BorderRadiusTweenTime = 0.1f;

    private Tween? _borderTween;

    private int _borderRadius;
    public int BorderRadius
    {
        get => _borderRadius;
        set
        {
            _borderRadius = value;
            _pressedStyle.BorderWidthBottom = value;
            _pressedStyle.BorderWidthTop = value;
            _pressedStyle.BorderWidthLeft = value;
            _pressedStyle.BorderWidthRight = value;
        }
    } 


    public override void _Ready()
    {
        SetLabelText();
        SetUnactive();
    }
    
    private void SetLabelText()
    {
        foreach (var @event in InputMap.ActionGetEvents(_direction.ToActionName()))
        {
            if (@event is InputEventKey keyEvent)
            {
                _inputLabel.Text = keyEvent.LocalizedKeyName();
                return;
            }
        }
    }

    public override void _UnhandledKeyInput(InputEvent @event)
    {
        if (@event is not InputEventKey key)
            return;

        if (_direction.IsJustPressed(key))
            SetPressed();
        else if (_direction.IsJustReleased(key))
            SetReleased();
    }

    private void SetPressed()
    {
        _borderTween?.Kill();
        _borderTween = CreateTween();
        _borderTween.TweenProperty(this, nameof(BorderRadius), MaxBorderRadius, BorderRadiusTweenTime);
    }

    private void SetReleased()
    {
        _borderTween?.Kill();
        _borderTween = CreateTween();
        _borderTween.TweenProperty(this, nameof(BorderRadius), 0, BorderRadiusTweenTime);
    }


    public void SetActive()
    {
        SetProcessUnhandledInput(true);
        _unactiveLayer.Hide();
    }

    public void SetUnactive()
    {
        SetProcessUnhandledInput(false);
        _unactiveLayer.Show();
        _borderTween?.Kill();
        BorderRadius = 0;
    }
}