using System.Linq;
using CombatCore.Primitives.Tags;
using Godot;
using Godot.Collections;

namespace WowGd.Src.Combat;

public partial class TagResource<T> : Resource
{
    [Export] public string Name { get; private set; } = string.Empty;
    public Tag<T> Tag => Tag<T>.Register(Name);
}

public static class TagResourceExt
{
    public static TagSet<T> IntoSet<T, [MustBeVariant] TResource>(
        this Array<TResource> tags)
        where TResource : TagResource<T>
    {
        return tags.Aggregate(
            default(TagSet<T>),
            (s, t) => s.With(t.Tag));
    }
}