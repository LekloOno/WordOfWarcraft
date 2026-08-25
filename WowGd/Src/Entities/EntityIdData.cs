using Godot;

namespace WowGd.Src.Entities;

/// <summary>
/// Data that identifies an entity.
/// </summary>
[GlobalClass]
public partial class EntityIdData : Resource
{
    /// <summary>
    /// A key that defines the type of this entity.
    /// For example, "tree", "goblin", etc.
    /// 
    /// It is not an id, as there might be multiple living entities of the same type.
    /// It can be used by the renderer to render specific things.
    /// </summary>
    [Export] public string Key {get; private set;} = null!;
}