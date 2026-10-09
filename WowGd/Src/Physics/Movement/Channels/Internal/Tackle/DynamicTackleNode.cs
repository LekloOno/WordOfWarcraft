using System.Diagnostics.CodeAnalysis;
using Godot;
using WowGd.Src.Entities;

namespace WowGd.Src.Physics.Movement.Channels.Internal.Tackle;

public partial class DynamicTackleNode : Node
{
    public readonly IEntity Entity;
    private readonly TackleNode _tackleNode;

    public float EffectiveLimit => _tackleNode.EffectiveLimit;
    public bool IsTackling => _tackleNode.IsTackling;
    public bool IsTackled => _tackleNode.IsTackled;

    // For godot binding only, should not be used.
    public DynamicTackleNode()
    {
        Entity = null!;
        _tackleNode = null!;
        SetPhysicsProcess(false);
    }

    public DynamicTackleNode(IEntity entity) : this()
    {
        Entity = entity;
        _tackleNode = new(entity);

        _tackleNode.TackleStarted   += OnTackleStarted;
        _tackleNode.TackleReleased  += OnTackleReleased;
        _tackleNode.GotTackled      += OnGotTackled;
        _tackleNode.GotReleased     += OnGotReleased;

        if (Entity is Node node)
            node.AddChild(this);
            
        SetPhysicsProcess(false);
    }

    public bool TryGetTackled([NotNullWhen(true)] out IEntity? tackled)
    {
        tackled = _tackleNode.Tackled?.Entity;
        return tackled != null;   
    }
}