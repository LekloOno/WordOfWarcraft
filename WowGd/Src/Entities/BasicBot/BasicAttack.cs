using Godot;
using WowGd.Src.Combat.Health;
using WowGd.Src.Tools;

namespace WowGd.Src.Entities.BasicBot;

[GlobalClass]
public partial class BasicAttack : Node, IEntityHealthHandler
{
    [Export] private float _attackRange = 1.5f;
    [Export] private float _attackCd    = 1f;
    [Export] private int _attackDmg     = 5;

    public bool Enabled => _enabled;
    private bool _enabled = true;

    private ITargetAcquirer _targetAcquirer = null!;
    private IEntity _entity = null!;

    public override void _Ready()
    {
        if (!this.TryGetSiblingComponent(out ITargetAcquirer? targetAcquirer))
            return;
        
        _targetAcquirer = targetAcquirer;

        if (this.TryGetComposedRecursive(out IEntity? entity))
            _entity = entity;
    }

    private float _acc;
    public override void _PhysicsProcess(double delta)
    {
        _acc += (float) delta;

        if (_targetAcquirer.Target is not IEntity target)
            return;

        if (_attackCd > _acc)
            return;

        if ((_entity.Body.Position - target.Body.Position).LengthSquared() > _attackRange * _attackRange)
            return;

        target.Health.Damage(_attackDmg);
        _acc = 0f;
    }

    public void OnDied()
    {
        SetPhysicsProcess(false);
    }

    public void OnDamaged(int hp) {}
    public void OnHealed(int hp) {}

    public void OnRessurected(int hp)
    {
        SetPhysicsProcess(true);
    }
}