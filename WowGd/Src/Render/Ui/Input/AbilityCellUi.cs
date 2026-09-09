using Godot;
using WowGd.Src.Combat.Abilities;
using WowGd.Src.Input;

namespace WowGd.Src.Render.Ui.Input;

[GlobalClass]
public partial class AbilityCellUi : Control, IAbilityLifeCycleHandler
{
    [Export] private TextureRect    _iconTexture = null!;
    [Export] private Label          _inputLabel = null!;
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

    public void OnCancelled()
    {
        
    }

    public void OnStarted()
    {
    }

    public void OnStopped()
    {
    }

    public void OnCoolDownStarted(ulong cooldown)
    {
    }

    public void OnCoolDownCancelled()
    {
    }
}
