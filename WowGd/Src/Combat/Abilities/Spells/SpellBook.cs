using System;
using System.Collections.Generic;
using CombatCore;
using CombatCore.Primitives;
using CombatCore.Sources;
using CombatCore.Sources.Attachment;
using CombatCore.Sources.Lifecycle;

namespace WowGd.Src.Combat.Abilities.Spells;

/// <summary>
/// The spell layer's way to build its nodes with nothing but the public authority API:
/// Entity > Ability > Run. Ability nodes are created on first use and forgotten when they leave the tree.
/// </summary>
public sealed class SpellBook
{
    readonly Authority _authority;
    readonly Dictionary<(EntityId, uint), Source> _abilities = [];

    public SpellBook(Authority authority)
    {
        _authority = authority;
        authority.OnFact<SourceTerminated>(f =>
        {
            if (f.Node.Attachment is AbilityAttachment a && _abilities.GetValueOrDefault((a.Caster, a.AbilityId)) == f.Node)
                _abilities.Remove((a.Caster, a.AbilityId));
            return Array.Empty<Draft>();
        });
    }

    /// <summary>The node of one ability of one caster (under the caster's node), created on first use.</summary>
    public Source Ability(EntityId caster, uint abilityId)
    {
        if (_abilities.TryGetValue((caster, abilityId), out var node)) return node;
        node = _authority.NewSource($"Ability {abilityId}", SourceTagSet.Of(SpellTags.Spell), new AbilityAttachment(abilityId, caster));
        _abilities[(caster, abilityId)] = node;
        _authority.Insert(_authority.EntitySource(caster), node);
        return node;
    }

    /// <summary>
    /// Starts one run: its node goes under the ability node. If the caster's node is gone the node is not inserted and
    /// every request from it is dropped ("source terminated").
    /// </summary>
    public SpellInstance Start(uint abilityId, EntityId caster, Source? parent = null)
    {
        var inst = new SpellInstance(_authority, abilityId, caster, parent);
        inst.Node = _authority.Spawn(Ability(caster, abilityId), $"Run of {abilityId}", SourceTagSet.Of(SpellTags.Instance), inst);
        return inst;
    }
}
