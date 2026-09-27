using Godot;
using WowGd.Src.Combat.Abilities.Launch.EffectsHolders.Effects.SingleAbilityEffects;
using WowGd.Src.Combat.Abilities.Launch.EffectsHolders.Effects.SingleAbilityEffects.Movement.External;
using WowGd.Src.Combat.Abilities.Launch.EffectsHolders.Effects.SingleAbilityEffects.Movement.External.Contribs;
using WowGd.Src.Entities;

namespace WowGd.Src.Combat.Abilities.Launch.EffectsHolders.Effects;

/// <summary>
/// Applies an impulse in the external channel of the target entities.
/// 
/// For gathered targets, the direction is the direction from the launcher to the target.
/// For the launcher, the direction is its wish dir.
/// </summary>
[GlobalClass, Tool]
public partial class ExternalMoveEffect : SingleAbilityEffect
{
    public override string Id => "effect_impulse";
    
    [Export] private ExternalMoveDirection _moveDirection = ExternalMoveDirection.Push;
    [Export] public ExternalMoveMode MoveMode
    {
        get => _moveMode;
        set
        {
            if (value == _moveMode)
                return;

            _moveMode = value;
            UpdateCurrent();
            NotifyPropertyListChanged();
        }
    }
    
    
    private ExternalMoveMode _moveMode;

    [Export] private ImpulseData      _impulseData = null!;
    [Export] private TranslationData  _translationData = null!;
    [Export] private AccelerationData _accelerationData = null!;

    private IMoveContribData _current = null!;

    public ExternalMoveEffect()
    {
        _impulseData = new ImpulseData();
        _translationData = new TranslationData();
        _accelerationData = new AccelerationData();
    }

    private void UpdateCurrent()
    {
        _current = _moveMode switch
        {
            ExternalMoveMode.Impulse => _impulseData,
            ExternalMoveMode.Translation => _translationData,
            ExternalMoveMode.Accelerate => _accelerationData,
            _ => throw new System.IndexOutOfRangeException(),
        };
    }

    public override void Effect(IEntity launcher, IEntity target, bool IsDirect, float gatherWeight, float actuateWeight)
    {
        Vector2 direction = (launcher.Body.GlobalPosition - target.Body.GlobalPosition).Normalized();
        if (_moveDirection is ExternalMoveDirection.Push)
            direction *= -1f;

        DoMove(target, direction, gatherWeight, actuateWeight);
    }

    public override void LauncherEffect(IEntity entity, float actuateWeight)
    {
        Vector2 direction = entity.WishDir.WishDir();
        if (_moveDirection is ExternalMoveDirection.Pull)
            direction *= -1f;

        DoMove(entity, direction, 1f, actuateWeight);
    }

    private void DoMove(IEntity target, Vector2 direction, float gatherWeight, float actuateWeight)
    {
        float weight = gatherWeight * actuateWeight;
        
        //if (_current == null)
        UpdateCurrent();

        _current!.Start(target.EntityMover, direction, weight);
    }

    public override void _ValidateProperty(Godot.Collections.Dictionary property)
    {
        switch (property["name"].AsString())
        {
            case nameof(_impulseData):
                SetPropertyVisible(
                    property,
                    _moveMode == ExternalMoveMode.Impulse);
                break;

            case nameof(_translationData):
                SetPropertyVisible(
                    property,
                    _moveMode == ExternalMoveMode.Translation);
                break;

            case nameof(_accelerationData):
                SetPropertyVisible(
                    property,
                    _moveMode == ExternalMoveMode.Accelerate);
                break;
        }
    }

    private static void SetPropertyVisible(
        Godot.Collections.Dictionary property,
        bool visible)
    {
        int usage = property["usage"].AsInt32();

        if (visible)
            usage |= (int)PropertyUsageFlags.Editor;
        else
            usage &= ~(int)PropertyUsageFlags.Editor;

        property["usage"] = usage;
    }
}