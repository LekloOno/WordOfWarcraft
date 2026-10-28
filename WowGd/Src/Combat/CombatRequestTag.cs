using CombatCore.Primitives.Tags;
using Godot;
using Godot.Collections;

namespace WowGd.Src.Combat;

[GlobalClass]
public partial class CombatRequestTag : TagResource<RequestTagDomain>;

public static class CombatRequestTagExt
{
    public static RequestTagSet IntoSet(this Array<CombatRequestTag> tags) =>
        tags.IntoSet<RequestTagDomain, CombatRequestTag>();
}