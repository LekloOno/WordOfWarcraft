using System;
using CombatCore;
using CombatCore.Primitives;
using WowGd.Src.Combat.Resources;
using WowGd.Src.Combat.Resources.FocusRes;
using WowGd.Src.Entities;
using WowGd.Src.Combat.Abilities.Targeting.Payload;
using WowGd.Src.Combat.Health;
using Godot;
using CombatCore.Sources;

namespace WowGd.Src.Combat.Statuses.Installs;

[GlobalClass]
public partial class TestStatusInstall : StatusInstall
{
    internal override IDisposable Build(StatusInstance inst, Authority auth)
    {
        RequestFilter filter = new(
            Origin: OriginMask.Original,
            Source: new(Owner: inst.Owner));

        //Source source = new(SourceKind.Status, inst.Id, inst.Owner);
        Source source = CombatCoreNode.Authority.Spawn(inst.Source, $"Run of test status", SourceTagSet.Of(SourceTag.Register("StatusInstance")));

        IEntity owner = EntitiesRegistryNode.Registry.FindAsEntity(inst.Owner)!;

        System.Collections.Generic.IEnumerable<Draft> fn(ResourceRequest r)
        {
            if (r.Resource != ResourceRegistry.GetId<IFocus>())
                return [];

            if (EntitiesRegistryNode.Registry.FindAsEntity(r.Target)?.GetRelationTo(owner) is not TargetRelation relation)
                return [];
                
            if (relation.IsEnemy())
                return [new ResourceDraft(source, r.Target, ResourceRegistry.GetId<IEntityHealth>(), ResourceDirection.Remove, r.Value.Resolve())];
            return [new ResourceDraft(source, r.Target, ResourceRegistry.GetId<IEntityHealth>(), ResourceDirection.Add, 5)];
        }

        return auth.Add(
            Rules.AddOn(filter, fn));
    }
}