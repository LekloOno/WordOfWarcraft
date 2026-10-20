using System;
using System.Collections.Generic;
using CombatCore;
using CombatCore.Primitives;
using WowGd.Src.Combat.Abilities.Targeting.Payload;
using WowGd.Src.Entities;

namespace WowGd.Src.Combat.Abilities.Mage;

public class MatterBreachBuff : IBreachBuff
{
    public string Key => "matter";

    private readonly Dictionary<IEntity, IDisposable> _handles = [];
    private static readonly TagSet _tagSet = TagSet.Of(Tag.Register("generate"));

    public void Apply(IEntity entity, TargetRelation targetRelation)
    {
        if (!targetRelation.IsSelf())
            return;

        if (_handles.TryGetValue(entity, out IDisposable? handle))
            handle.Dispose();

        _handles.Add(entity, 
            CombatCoreNode.Authority.Add(Rules.Mod(
                new(direction: ResourceDirection.Add,
                    Kind: SourceKind.Ability,
                    Origin: OriginMask.Original,
                    Target: entity.Id,
                    SourceOwner: entity.Id,
                    RequireAll: _tagSet),
                ValueMod.Times(1.25f))));
    }

    public void Cancel(IEntity entity, TargetRelation targetRelation)
    {
        if (_handles.Remove(entity, out IDisposable? handle))
            handle.Dispose();
    }
}