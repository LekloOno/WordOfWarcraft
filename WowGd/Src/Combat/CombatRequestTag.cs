using System.Linq;
using CombatCore.Primitives;
using Godot;
using Godot.Collections;

namespace WowGd.Src.Combat;

[GlobalClass]
public partial class CombatRequestTag : Resource
{
    [Export] public string Name { get; private set; } = string.Empty;
    public Tag Tag => Tag.Register(Name);
}

public static class CombatRequestTagExt
{
    public static TagSet IntoSet(this Array<CombatRequestTag> tags) =>
        tags.Aggregate(default(TagSet), (s, t) => s.With(t.Tag)); 
}