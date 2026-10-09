using System;
using System.Diagnostics.CodeAnalysis;
using Godot;
using WowGd.Src.Entities;

namespace WowGd.Src.Physics.Movement.Channels.Internal.Tackle;

public partial class DynamicTackleNode : Node
{
    public float EffectiveLimit => _tackleNode.EffectiveLimit;
    public bool IsTackling => _tackleNode.IsTackling;
    public bool IsTackled => _tackleNode.IsTackled;
    public bool TryGetTackled([NotNullWhen(true)] out IEntity? tackled)
    {
        tackled = _tackleNode.Tackled?.Entity;
        return tackled != null;   
    }

    /// <summary>
    /// Occurs when this node starts tackling another node.
    /// </summary>
    public event Action<DynamicTackleNode>? TackleStarted;
    /// <summary>
    /// Occurs when this node stops tackling another node.
    /// </summary>
    public event Action<DynamicTackleNode>? TackleReleased;
    
    /// <summary>
    /// Occurs when another node starts tackling this node.
    /// </summary>
    /// <param name="tackler">The node that started tackling this node.</param>
    /// <param name="tacklerCount">
    /// The number of nodes currently tackling this node.
    /// </param>
    public event Action<DynamicTackledEventArgs>? GotTackled;
    /// <summary>
    /// Occurs when another node stops tackling this node.
    /// </summary>
    /// <param name="tackler">The node that stopped tackling this node.</param>
    /// <param name="tacklerCount">
    /// The number of nodes currently tackling this node.
    /// </param>
    public event Action<DynamicTackledEventArgs>? GotReleased;

    private void OnTackleStarted(TackleNode node) =>
        TackleStarted?.Invoke(node.Entity.EntityMover.DynamicTackleNode);

    private void OnTackleReleased(TackleNode node)
    {
        Entity.EntityMover.Internal.ReleaseOverride();
        SetPhysicsProcess(false);
        TackleReleased?.Invoke(node.Entity.EntityMover.DynamicTackleNode);
    }

    private void OnGotTackled(TackledEventArgs args) =>
        GotTackled?.Invoke(new(args));

    private void OnGotReleased(TackledEventArgs args) =>
        GotReleased?.Invoke(new(args));
}