using Godot;
using WowGd.Src.Combat.Abilities.Targeting.Player;
using WowGd.Src.Entities;
using WowGd.Src.Render.Animation.Followers;
using WowGd.Src.Render.Camera;
using WowGd.Src.Render.Ui.Input.Impls;
using WowGd.Src.Render.Ui.Input.Impls.MoveMode;
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

    [Export]
    private MoveModeUi? _moveModeUi;

    [Export]
    private RadiusFollower _radiusFollower = null!;

    [Export]
    private DamageShakesNode? _damageShakesNode;

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

        if (_moveModeUi != null)
            _moveModeUi.MoveModeInput = clientEntity.MoveModeInput;

        if (entity.Body is Node2D bodyNode)
            _radiusFollower.Node = bodyNode;

        _damageShakesNode?.Init(entity.Health);

        return base.InitSpec(entity);
    }

    public override void _Ready()
    {
        PlayerInputUiSyncer.Sync();
    }
}
