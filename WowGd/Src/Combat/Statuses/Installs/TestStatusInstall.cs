using System;
using CombatCore;
using CombatCore.Primitives;
using WowGd.Src.Combat.Resources;
using WowGd.Src.Combat.Resources.FocusRes;
using WowGd.Src.Entities;
using WowGd.Src.Combat.Abilities.Targeting.Payload;
using WowGd.Src.Combat.Health;
using Godot;

namespace WowGd.Src.Combat.Statuses.Installs;

[GlobalClass]
public partial class TestStatusInstall : StatusInstall
{
    internal override IDisposable Build(StatusInstance inst, Authority auth)
    {
        RequestFilter filter = new(
            Origin: OriginMask.Original,
            SourceOwner: inst.Owner);

        Source source = new(SourceKind.Status, inst.Id, inst.Owner);

        IEntity owner = EntitiesRegistryNode.Registry.FindAsEntity(inst.Owner)!;

        System.Collections.Generic.IEnumerable<Draft> fn(ResourceRequest r)
        {
            if (r.Resource != ResourceRegistry.GetId<IFocus>())
                return [];

            if (EntitiesRegistryNode.Registry.FindAsEntity(r.Target)?.GetRelationTo(owner) is not TargetRelation relation)
                return [];
                
            if (relation.IsEnemy())
                return [new RequestDraft(source, r.Target, ResourceRegistry.GetId<IEntityHealth>(), Role.Drain, r.Value.Resolve())];
            return [new RequestDraft(source, r.Target, ResourceRegistry.GetId<IEntityHealth>(), Role.Feed, 5)];
        }

        return CombatCoreNode.Authority.Add(
            Rules.AddOn(filter, fn));
    }
}