using Godot;
using WowGd.Src.Combat.Health;
using WowGd.Src.Physics.Movement.Channels.Internal.Tackle;
using WowGd.Src.Tools;

namespace WowGd.Src.Entities.BasicBot;

[GlobalClass]
public partial class BasicTackle : Node, IEntityHealthHandler
{
    [Export] private float _tackleSpeed = 1.5f;
    [Export] private float _tackleDuration = 2f;
    [Export] private float _staleDuration = 3f;

    public bool Enabled => _enabled;
    private bool _enabled = true;

    private IEntity _entity = null!;

    private ITargetAcquirer _targetAcquirer = null!;

    private DynamicTackleNode DynamicTackleNode => _entity.EntityMover.DynamicTackleNode;

    public override async void _Ready()
    {
        SetPhysicsProcess(false);

        if (this.TryGetComposedRecursive(out IEntity? entity))
            _entity = entity;

        if (this.TryGetSiblingComponent(out ITargetAcquirer? targetAcquirer))
            _targetAcquirer = targetAcquirer;

        await _entity.Initialization;
        SetPhysicsProcess(true);
    }

    private float _acc;

    public override void _PhysicsProcess(double delta)
    {
        if (!DynamicTackleNode.IsTackling)
            CheckTackle((float) delta);
    }

    private void CheckTackle(float dt)
    {
        _acc += dt;
        if (_acc < _staleDuration)
            return;

        if (_targetAcquirer.IsProcessTick() &&
            _targetAcquirer.Target is not null &&
            DynamicTackleNode.StartTackle(_targetAcquirer.Target, _tackleSpeed))
            _acc = 0f;
    }

    private void CheckRelease(float dt)
    {
        _acc += dt;

        if (_acc < _tackleDuration)
            return;

        DynamicTackleNode.ReleaseTackle();
        _acc = 0f;
    }

    public void OnDied()
    {
        SetPhysicsProcess(false);
    }

    public void OnConsumed(int hp) {}
    public void OnGenerated(int hp) {}

    public void OnResurrected(int hp)
    {
        SetPhysicsProcess(true);
    }
}