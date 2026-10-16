using CombatCore;
using Godot;
using Godot.Collections;
using WowGd.Src.Entities;

namespace WowGd.Src.Combat.Statuses;

[GlobalClass]
public partial class StatusData : Resource
{
    private static uint _count;

    [Export] private string _name = string.Empty;
    [Export] private StatusCategory _category;
    [Export] private Array<CombatRequestTag> _requestTags = [];
    [Export] private StatusOp _statusOp;

    [Export] private LevelModelData? _levelModelData;
    [Export] private TimeModelData? _timeModelData;

    [Export] private StatusEffects? _effects = null;
    [Export] private StatusInstalls? _installs = null;

    private StatusDefinition? Definition;

    public void Submit(IEntity owner, IEntity target, long level = 1, ulong? duration = null)
    {
        Definition ??= new(
            Id: _count ++,
            Name: _name,
            Category: _category,
            Grants: _requestTags.IntoSet(),
            Level: _levelModelData?.Build(),
            Time: _timeModelData?.Build(),
            Effects: _effects?.Build(),
            Install: _installs?.Build());

        CombatCoreNode.Authority.Submit(new StatusDraft(
            new CombatCore.Primitives.Source(CombatCore.Primitives.SourceKind.Status, Definition.Id, owner.Id),
            target.Id,
            _statusOp,
            Definition,
            level,
            duration
        ));
    }
}