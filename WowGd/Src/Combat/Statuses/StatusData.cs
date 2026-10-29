using CombatCore;
using CombatCore.Primitives.Tags;
using CombatCore.Sources;
using Godot;
using Godot.Collections;
using WowGd.Src.Entities;

namespace WowGd.Src.Combat.Statuses;

[GlobalClass]
public partial class StatusData : Resource
{
    [Export] private string _name = string.Empty;
    [Export] private StatusCategory _category;
    [Export] private Array<CombatGrantTag> _requestTags = [];

    [Export] private LevelModelData? _levelModelData;
    [Export] private TimeModelData? _timeModelData;

    [Export] private StatusEffects? _effects = null;
    [Export] private StatusInstalls? _installs = null;

    private StatusDefinition? Definition;
    private readonly System.Collections.Generic.Dictionary<IEntity, Source> _sources = [];

    private Source StatusSource(IEntity entity)
    {
        if (_sources.TryGetValue(entity, out var node))
            return node;

        node = CombatCoreNode.Authority.NewSource($"Status {_name}", SourceTagSet.Of(SourceTag.Register("Status")));
        _sources[entity] = node;
        CombatCoreNode.Authority.Insert(CombatCoreNode.Authority.EntitySource(entity.Id), node);
        return node;
    }

    public void Submit(IEntity owner, IEntity target, StatusOp statusOp, long level = 1, long? duration = null)
    {
        Definition ??= new(
            Name: _name,
            Category: _category,
            Grants: _requestTags.IntoSet(),
            Level: _levelModelData?.Build(),
            Time: _timeModelData?.Build(),
            Effects: _effects?.Build(),
            Install: _installs?.Build());

        CombatCoreNode.Authority.Submit(new StatusDraft(
            StatusSource(owner),
            target.Id,
            statusOp,
            Definition,
            level,
            duration
        ));
    }
}