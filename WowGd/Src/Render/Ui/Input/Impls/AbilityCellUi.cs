using Godot;
using WowGd.Src.Combat.Abilities;
using WowGd.Src.Combat.Abilities.CoolDowns.EventData;
using WowGd.Src.Combat.Abilities.Data;
using WowGd.Src.Combat.Abilities.Handlers;
using WowGd.Src.Input;

namespace WowGd.Src.Render.Ui.Input.Impls;

[GlobalClass]
public partial class AbilityCellUi : Control,
    IAbilityHandler,
    ICoolDownHandler
{
    [Export] private TextureRect    _iconTexture    = null!;
    [Export] private Label          _inputLabel     = null!;
    [Export] private ColorRect      _unactiveLayer  = null!;
    [Export] private Range          _cooldownLayer  = null!;

    private Tween? _cooldownTween;
    private IAbility _ability = null!;
    public IAbility Ability
    {
        get => _ability;
        set
        {
            if (_ability == value)
                return;

            if (_ability != null)
                this.TryUnbindAll(_ability);

            _ability = value;
            this.TryBindAll(_ability);

            _iconTexture.Texture = _ability.Data.Icon;
        }
    }

    public AbilityCellUi() { }
    public AbilityCellUi(IAbility ability)
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
        SetPhysicsProcess(false);
    }

    public void SetActive()
    {
        _unactiveLayer.Hide();
        _inputLabel.Show();
        SetPhysicsProcess(_ability.Data.StartPreconditions.Count > 0);
    }

    public void SetUnactive()
    {
        _inputLabel.Hide();
        _unactiveLayer.Show();
        SetPhysicsProcess(false);
    }

    public void OnCdStartedAt(CoolDownEventData cdData)
    {
        StartTweenFor(cdData.Base, cdData.Effective);
    }

    private void StartTweenFor(ulong @base, ulong effective)
    {
        _cooldownTween?.Kill();
        _cooldownLayer.Value = Mathf.Min(effective / @base, 1f);

        _cooldownTween = CreateTween();
        _cooldownTween.TweenProperty(_cooldownLayer, "value", 0f, effective / 1000f);
    }

    public void OnCdCompleted(CoolDownCompletion completion)
    {
        _cooldownTween?.Kill();
        _cooldownLayer.Value = 0f;
    }

    public void OnCdReduced(CoolDownModification cdReduction)
    {
        StartTweenFor(cdReduction.Base, cdReduction.Effective);
    }

    public void OnCdEnlengthed(CoolDownModification cdElongation)
    {
        StartTweenFor(cdElongation.Base, cdElongation.Effective);
    }

    public override void _PhysicsProcess(double delta)
    {
        // LAZY ALERT - client info entity, it's late, im tired, i should have a proper referencing instead
        // Besides, we should later find a better event based mechanism.
        _unactiveLayer.Visible = !_ability.Data.StartPreconditions.CheckAll(ClientInfo.Entity);
    }

    public void OnCancelled() { }
    public void OnStarted() { }
    public void OnStopped() { }

    public void OnTargetingStarted() { }
    public void OnTargetingCompleted() { }
}
