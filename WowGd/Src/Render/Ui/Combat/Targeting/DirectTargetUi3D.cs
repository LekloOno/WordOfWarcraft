using System;
using Godot;
using WowGd.Src.Entities;
using WowGd.Src.Input;
using WowGd.Src.Input.Targeting.Direct;

namespace WowGd.Src.Render.Ui.Combat.Targeting;

/// <summary>
/// Render-side targeting UI for an entity represented in a 3D scene.
///
/// Tracks a 3D position, projects it into screen space, and exposes
/// itself to the DirectTargetEntitiesManager when visible.
/// </summary>
[GlobalClass]
public partial class DirectTargetUi3D : Control, IDirectTargetUi
{
    public IEntity Entity { get; set; } = null!;
    [Export] private VisibleOnScreenNotifier3D _visibilityNotifier = null!;
    [Export] private Node3D _position = null!;
    [Export] private Label _indexLabel = null!;
    [Export] private Color _validColor;
    [Export] private Color _unvalidColor;
    [Export] private Node3D? _selectedRing;

    private Camera3D _camera = null!;
    
    public event Action<IDirectTargetUi>? ScreenEntered;
    public event Action<IDirectTargetUi>? ScreenExited;

    public override void _EnterTree()
    {
        DirectTargetEntitiesManager.Register(this);
        Visible = false;
        SetProcess(false);
        _selectedRing?.Hide();
    }

    public override void _Ready()
    {
        _camera = GetViewport().GetCamera3D();
        _visibilityNotifier.ScreenEntered += OnScreenEntered;
        _visibilityNotifier.ScreenExited += OnScreenExited;
        SetProcess(false);

        _indexLabel.TopLevel = true;
    }

    private void OnScreenEntered() => ScreenEntered?.Invoke(this);
    private void OnScreenExited() => ScreenExited?.Invoke(this);
    
    public override void _ExitTree()
    {
        DirectTargetEntitiesManager.Unregister(this);
    }

    public override void _Process(double delta)
    {
        UpdateScreenPosition();
    }

    public bool Enable()
    {
        Visible = true;
        SetProcess(true);

        return true;
    }

    public void UpdateIndex(int index)
    {
        if (index.TryGetAbilityFirstKey(out string key))
            _indexLabel.Text = key;
        else
            _indexLabel.Text = string.Empty;
    }

    public void UpdateValidity(bool valid)
    {
        Modulate = valid
            ? _validColor
            : _unvalidColor;
    }

    public void Disable()
    {
        Visible = false;
        SetProcess(false);
    }

    private void UpdateScreenPosition()
    {
        Vector3 worldPosition = 
            _position.GetGlobalTransformInterpolated().Origin;

        Vector2 screenPosition = 
            (_camera ??= GetViewport().GetCamera3D()).UnprojectPosition(worldPosition);

        _indexLabel.GlobalPosition = screenPosition - Size * 0.5f;
    }

    public void Select() =>
        _selectedRing?.Show();
    
    public void Unselect() =>
        _selectedRing?.Hide();
}