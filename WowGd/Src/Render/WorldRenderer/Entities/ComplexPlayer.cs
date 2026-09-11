using Godot;
using WowGd.Src.Combat.Abilities.Targeting.Player;
using WowGd.Src.Entities;
using WowGd.Src.Render.Ui.Input.Impls;
using WowGd.Src.Render.Ui.Input.Impls.Worder;

namespace WowGd.Src.Render.WorldRenderer.Entities;

public partial class ComplexPlayer : SimpleDude
{
    [Export]
    private WorderDisplay _worder = null!;

    [Export]
    private SelectionCombatModeUi? _selectionCombatModeUi;

    [Export]
    private DirectTargetUi? _directTargetModeUi;

    public override bool InitSpec(IEntity entity)
    {
        if (entity is not IClientEntity clientEntity)
            return false;

        _worder.Driver = clientEntity.DactyloDriver;

        if (_selectionCombatModeUi != null)
            _selectionCombatModeUi.SelectionModeInput = clientEntity.SelectionModeInput;

        if (_directTargetModeUi != null)
        {
            if (clientEntity.TargetIntentDriver is not PlayerTargetIntentDriver pDriver)
                return false;

            _directTargetModeUi.TargetDriver = pDriver;
        }

        return base.InitSpec(entity);
    }

    public override void _Ready()
    {
        PlayerInputUiSyncer.Sync();
    }
}
