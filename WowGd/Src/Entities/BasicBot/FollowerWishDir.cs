using System.Diagnostics.CodeAnalysis;
using Godot;
using WowGd.Src.Physics.Movement.WishDir;
using WowGd.Src.Tools;

namespace WowGd.Src.Entities.BasicBot;

[GlobalClass]
public partial class FollowerWishDir : Node, IWishDir
{
    private static uint _count = 0;

    private readonly uint _id = _count ++;
    [Export(PropertyHint.Layers2DPhysics)] private uint _targetTeamMask; 
    public bool Enabled => _enabled;
    private bool _enabled = true;
    private IEntity _entity = null!;
    private IEntity? _target;

    public override void _Ready()
    {
        if (this.TryGetComposedRecursive(out IEntity? entity))
            _entity = entity;
    }

    private const uint TargetUpdateMask = 15;
    public override void _PhysicsProcess(double delta)
    {
        bool processTick =
            (TargetUpdateMask & Engine.GetPhysicsFrames()) ==
            (TargetUpdateMask & _id);

        if (processTick)
            TryGetClosestTarget(out _target);
    }

    public bool Disable()
    {
        throw new System.NotImplementedException();
    }

    public bool Enable()
    {
        throw new System.NotImplementedException();
    }

    public Vector2 WishDir()
    {
        if (_target == null)
            return Vector2.Zero;

        return (_target.Body.Position - _entity.Body.Position).Normalized();
    }

    private bool TryGetTarget([NotNullWhen(true)] out IEntity? target)
    {
        foreach (IEntity entity in EntitiesRegistry.Entities)
        {
            if (entity == _entity)
                continue;

            if ((entity.TeamMask & _targetTeamMask) != 0)
            {
                target = entity;
                return true;
            }
        }

        target = null;
        return false;
    }

    private bool TryGetClosestTarget([NotNullWhen(true)] out IEntity? target)
    {
        target = null;
        float closestSquaredDist = float.PositiveInfinity;

        foreach (IEntity entity in EntitiesRegistry.Entities)
        {
            if (entity == _entity)
                continue;

            if ((entity.TeamMask & _targetTeamMask) != 0)
            {
                float squaredDist = (entity.Body.Position - _entity.Body.Position).LengthSquared();
                if (squaredDist < closestSquaredDist)
                {
                    target = entity;
                    closestSquaredDist = squaredDist;
                }
            }
        }

        return target != null;
    }
}