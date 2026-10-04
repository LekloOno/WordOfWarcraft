using System.Threading;
using System.Threading.Tasks;
using Godot;
using WowGd.Src.Combat.Abilities.Data;
using WowGd.Src.Combat.Abilities.Targeting;
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

    private IEntity _entity = null!;

    public override void _Ready()
    {
        if (this.TryGetComposedRecursive(out IEntity? entity))
            _entity = entity;
    }

    private float _acc;
    public override void _PhysicsProcess(double delta)
    {
        _acc += (float) delta;

        if (_attackCd <= _acc)
            StartAttack();            
    }

    private CancellationTokenSource? _cts;
    private async void StartAttack()
    {
        SetPhysicsProcess(false);

        _cts?.Cancel();
        _cts = new();

        CancellationToken token = _cts.Token;

        IEntity? target;
        TargetResult result = await _entity.TargetIntentDriver.RetrieveTargetIntent(_entity, TargetIntentAcquirer.Melee, token);

        while(!result.TryGet(out TargetIntent intent) || !intent.TryGetEntity(out target) /*||
            _entity.DistanceSquaredTo(target) > _attackRange * _attackRange*/)
        {
            await Task.Delay(500);
            
            if (token.IsCancellationRequested)
                return;

            result = await _entity.TargetIntentDriver.RetrieveTargetIntent(_entity, TargetIntentAcquirer.Melee, token);
        }

        target.Health.Consume(_attackDmg, out _);
        _acc = 0f;

        _cts = null;

        SetPhysicsProcess(true);
    }

    public void OnDied()
    {
        SetPhysicsProcess(false);
        _cts?.Cancel();
    }

    public void OnConsumed(int hp) {}
    public void OnGenerated(int hp) {}

    public void OnResurrected(int hp)
    {
        SetPhysicsProcess(true);
    }
}