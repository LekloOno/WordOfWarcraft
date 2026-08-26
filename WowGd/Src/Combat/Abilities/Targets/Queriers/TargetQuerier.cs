using System.Threading.Tasks;
using Godot;
using WowGd.Src.Combat.Abilities.Targets.AreaGatherers;
using WowGd.Src.Combat.Abilities.Targets.Payload;
using WowGd.Src.Entities;
using WowGd.Src.Tools;

namespace WowGd.Src.Combat.Abilities.Targets.Queriers;

[GlobalClass]
public abstract partial class TargetQuerier : Node, ITargetQuerier
{
    protected AreaGatherer? _areaGatherer;

    public override sealed void _Ready()
    {
        this.TryGetComponent(out _areaGatherer);
        ReadySpec();
    }

    public abstract Task<TargetsPayload> Query(IEntity requester);
    protected virtual void ReadySpec() {}
}