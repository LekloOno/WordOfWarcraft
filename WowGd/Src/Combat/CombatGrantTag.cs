using CombatCore.Primitives.Tags;
using Godot;
using Godot.Collections;

namespace WowGd.Src.Combat;

[GlobalClass]
public partial class CombatGrantTag : TagResource<GrantTagDomain>;

public static class CombatGrantTagExt
{
    public static GrantTagSet IntoSet(this Array<CombatGrantTag> tags) =>
        tags.IntoSet<GrantTagDomain, CombatGrantTag>();
}