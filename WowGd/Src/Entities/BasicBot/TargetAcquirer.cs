using Godot;
using WowGd.Src.Tools;

namespace WowGd.Src.Entities.BasicBot;

[GlobalClass]
public partial class TargetAcquirer : Node, ITargetAcquirer
{
    private static uint _count = 0;

    private readonly uint _id = _count ++;
    [Export] private TeamMask _targetTeamMask; 
    public bool Enabled => _enabled;

    private bool _enabled = true;
    private IEntity _entity = null!;

    public IEntity? Target => _target;
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
            _entity.TryGetClosestTarget(out _target, _targetTeamMask);
    }
}