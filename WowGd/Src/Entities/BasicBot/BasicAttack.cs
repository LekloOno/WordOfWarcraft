using System.Threading;
using System.Threading.Tasks;
using CombatCore;
using CombatCore.Primitives;
using CombatCore.Sources;
using Godot;
using WowGd.Src.Combat;
using WowGd.Src.Combat.Abilities.Data;
using WowGd.Src.Combat.Abilities.Spells;
using WowGd.Src.Combat.Abilities.Targeting;
using WowGd.Src.Combat.Health;
using WowGd.Src.Combat.Resources;
using WowGd.Src.Tools;

namespace WowGd.Src.Entities.BasicBot;

[GlobalClass]
public partial class BasicAttack : Node, IEntityHealthHandler
{
    [Export] private float _attackRange = 1.5f;
    [Export] private float _attackCd    = 1f;
    [Export] private int _attackDmg     = 5;
    [Export] private TargetIntentAcquirer _targetType = TargetIntentAcquirer.Melee;

    public bool Enabled => _enabled;
    private bool _enabled = true;

    private IEntity _entity = null!;
    private Source? _source;

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
    private static Authority Authority => CombatCoreNode.Authority;

    /// <summary>
    /// Starts one run: its node goes under the ability node. If the caster's node is gone the node is not inserted and
    /// every request from it is dropped ("source terminated").
    /// </summary>
    public SpellInstance Start()
    {
        var inst = new SpellInstance(Authority, 0, _entity.Id, null);
        inst.Node = Authority.Spawn(_source!, $"Run of basic attack", SourceTagSet.Of(SpellTags.Instance), inst);
        return inst;
    }

    private async void StartAttack()
    {
        _source ??= CombatCoreNode.Authority.Spawn(CombatCoreNode.Authority.EntitySource(_entity.Id), "basic attack", SourceTagSet.Of(SpellTags.Spell));
        

        SetPhysicsProcess(false);

        _cts?.Cancel();
        _cts = new();

        CancellationToken token = _cts.Token;

        IEntity? target;
        TargetResult result = await _entity.TargetIntentDriver.RetrieveTargetIntent(_entity, _targetType, token);

        while(!result.TryGet(out TargetIntent intent) ||
            !intent.TryGetEntity(out target) ||
            !InRange(intent))
        {
            await Task.Delay(500);
            
            if (token.IsCancellationRequested)
                return;

            result = await _entity.TargetIntentDriver.RetrieveTargetIntent(_entity, _targetType, token);
        }

        var inst = Start();

        CombatCoreNode.Authority.Submit(new ResourceDraft(
            inst.Node,
            target.Id,
            ResourceRegistry.GetId<IEntityHealth>(),
            ResourceDirection.Remove,
            Mathf.FloorToInt(_attackDmg)));

        inst.Stop(StopReason.Completed);

        _acc = 0f;

        _cts = null;

        SetPhysicsProcess(true);
    }

    private bool InRange(TargetIntent intent)
    {
        return _targetType switch
        {
            TargetIntentAcquirer.Self => true,
            TargetIntentAcquirer.Melee => true,
            TargetIntentAcquirer.Direct =>
                intent.TryGetEntity(out IEntity? target) &&
                _entity.DistanceSquaredTo(target) <= _attackRange * _attackRange,
            TargetIntentAcquirer.Free =>
                intent.Position.DistanceSquaredTo(_entity.Body.GlobalPosition) > _attackRange * _attackRange,
            _ => throw new System.ArgumentOutOfRangeException(nameof(intent)),
        };
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