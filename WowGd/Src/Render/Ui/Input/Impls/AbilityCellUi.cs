using Godot;
using WowGd.Src.Combat.Abilities;
using WowGd.Src.Input;

namespace WowGd.Src.Render.Ui.Input.Impls;

[GlobalClass]
public partial class AbilityCellUi : Control, IAbilityLifeCycleHandler
{
    [Export] private TextureRect    _iconTexture    = null!;
    [Export] private Label          _inputLabel     = null!;
    [Export] private ColorRect      _unactiveLayer  = null!;
    [Export] private Range          _cooldownLayer  = null!;

    private Tween? _cooldownTween;
    private IListenableAbility _ability = null!;
    public IListenableAbility Ability
    {
        get => _ability;
        set
        {
            if (_ability == value)
                return;

            if (_ability != null)
                this.Unbind(_ability);

            _ability = value;
            this.Bind(_ability);

            _iconTexture.Texture = _ability.Data.Icon;
        }
    }

    public AbilityCellUi() { }
    public AbilityCellUi(IListenableAbility ability)
    {
        Ability = ability;
    }

    public void SetIndex(int abilityIndex)
    {
        if (abilityIndex.TryGetAbilityFirstKey(out string key))
            _inputLabel.Text = key;
    }

    public void SetInput(string actionName)
    {
        foreach (var @event in InputMap.ActionGetEvents(actionName))
        {
            if (@event is InputEventKey keyEvent)
            {
                _inputLabel.Text = keyEvent.LocalizedKeyName();
                return;
            }
        }
    }

    public override void _Ready()
    {
        _cooldownLayer.Value = 0f;
    }

    public void SetActive()
    {
        _unactiveLayer.Hide();
        _inputLabel.Show();
    }

    public void SetUnactive()
    {
        _inputLabel.Hide();
        _unactiveLayer.Show();
    }

    public void OnCancelled() { }
    public void OnStarted() { }
    public void OnStopped() { }

    public void OnCoolDownStarted(ulong cooldown)
    {
        _cooldownTween?.Kill();

        _cooldownLayer.Value = 1f;

        _cooldownTween = CreateTween();
        _cooldownTween.TweenProperty(_cooldownLayer, "value", 0f, cooldown / 1000f);
    }

    public void OnCoolDownCancelled()
    {
        _cooldownTween?.Kill();

        _cooldownLayer.Value = 0f;
    }
}
