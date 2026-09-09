using Godot;
using WowGd.Src.Combat.Abilities;

namespace WowGd.Src.Render.Ui.Input;

[GlobalClass]
public partial class AbilityCellUi : Control, IAbilityLifeCycleHandler
{
    private IListenableAbility _ability = null!;

    public AbilityCellUi() { }
    public AbilityCellUi(IListenableAbility ability)
    {
        _ability = ability;
        this.Bind(ability);
    }

    public void OnCancelled()
    {
        throw new System.NotImplementedException();
    }

    public void OnStarted()
    {
        throw new System.NotImplementedException();
    }

    public void OnStopped()
    {
        throw new System.NotImplementedException();
    }
}
