using System;
using System.Buffers;
using System.Collections.Generic;
using WowGd.Src.Entities;

namespace WowGd.Src.Physics.Movement.Channels.Internal.Tackle;


/// <summary>
/// Represents the tackling relationship between entities and maintains the
/// speed limit propagated through those relationships.
/// </summary>
/// <remarks>
/// <para>
/// A <see cref="TackleNode"/> is associated with an entity. Nodes form a directed
/// graph representing which entities are currently tackling which other entities.
/// </para>
/// 
/// <para>
/// The tackling graph follows these rules:
/// </para>
/// 
/// <list type="bullet">
/// <item>
///     Tackling is 0..1 - A node can tackle at most one other node.
/// </item>
/// <item>
///     Tackled is 0..n - A node can be tackled by any number of other nodes.
/// </item>
/// <item>
///     An entity's maximum speed is limited by the most restrictive limit
///     imposed by itself (internal) or any of its ancestors in the graph.
/// </item>
/// <item>
///     Limits are transitive. If A tackles B and B tackles C, then C is
///     subject to the most restrictive limit imposed by A and B.
/// </item>
/// <item>
///     Limits are not propagated through cycles. A cycle contains no node
///     that can independently initiate movement, so the effective limit
///     within a cycle is not constrained by the cycle itself.
/// </item>
/// </list>
/// 
/// <para>
/// The graph is represented as a directed graph.
/// Since access to limits are likely to be much more frequent than topology
/// or internal limits update, the limits are cached instead of computed recursively.
/// Cache updates are propagated selectively so that a topology or limit change
/// only affects nodes whose effective limit may have changed.
/// </para>
/// 
/// <para>
/// Each node maintains two components of its effective limit:
/// </para>
/// <list type="bullet">
/// <item>
///     The node's own limit, exposed by <see cref="InternalLimit"/>.
/// </item>
/// <item>
///     The most restrictive effective limit propagated from its tackling parents.
/// </item>
/// </list>
/// 
/// <para>
/// When a node's own limit changes, propagation is only necessary when the
/// change can alter its effective limit. In particular, a change must be
/// propagated when the previous own limit was the effective limit and becomes
/// less restrictive, or when the new own limit becomes more restrictive than
/// the previous effective limit.
/// </para>
/// 
/// <para>
/// When a parent's effective limit changes or a parent relationship is added
/// or removed, the cached parent limit is updated selectively:
/// </para>
/// <list type="number">
/// <item>
///     If the changed or removed parent was not responsible for the cached
///     parent limit - it was not the most restrictive - no update is required.
/// </item>
///     If a new or more restrictive parent limit becomes the cached parent
///     limit, the cache can be updated directly from this limit.
///     Propagation is only required if this also changes the node's effective limit.
/// <item>
///     If the removed parent was responsible for the cached parent limit, the
///     remaining parents must be inspected to determine the new cached limit.
///     Propagation is only required if the resulting effective limit changes.
/// </item>
/// </list>
/// </remarks>
public sealed partial class TackleNode(IEntity entity)
{
    public IEntity Entity => entity;
    /// <summary>
    /// The limit imposed by this node on the node it is tackled.
    /// </summary>
    /// <remarks>
    /// This value is <see cref="float.PositiveInfinity"/> when the node is not
    /// currently tackling another node.
    /// </remarks>
    public float InternalLimit { get; private set; } = float.PositiveInfinity;
    /// <summary>
    /// The effective limit imposed by this node.
    /// </summary>
    /// <remarks>
    /// The effective limit is the most restrictive limit between <see cref="InternalLimit"/>
    /// and the limits propagated from this node's ancestor's.
    /// </remarks>
    public float EffectiveLimit { get; private set; } = float.PositiveInfinity;

    /// <summary>
    /// Whether this node is part of graph cycle.
    /// </summary>
    private bool _cycling = false;
    /// <summary>
    /// This node's active tacklers.
    /// </summary>
    private readonly List<TackleNode> _tacklers = [];
    /// <summary>
    /// This node's tackled target.
    /// </summary>
    public TackleNode? Tackled { get; private set; } = null;
    /// <summary>
    /// A cached limit computed from this node's parents.
    /// </summary>
    private float _parentsLimit = float.PositiveInfinity;

    /// <summary>
    /// Indicates whether this node is currently tackling another node.
    /// </summary>
    /// <returns>
    /// <see langword="true"/> if this node is currently tackling another node;
    /// otherwise, <see langword="false"/>.
    /// </returns>
    public bool IsTackling => Tackled is not null;
    /// <summary>
    /// Indicates whether this node is currently being tackled by at least one other node.
    /// </summary>
    /// <returns>
    /// <see langword="true"/> if one or more nodes are currently tackling this node;
    /// otherwise, <see langword="false"/>.
    /// </returns>
    public bool IsTackled => _tacklers.Count != 0;

    /// <summary>
    /// Occurs when this node starts tackling another node.
    /// </summary>
    public event Action<TackleNode>? TackleStarted;
    /// <summary>
    /// Occurs when this node stops tackling another node.
    /// </summary>
    public event Action<TackleNode>? TackleReleased;
    
    /// <summary>
    /// Occurs when another node starts tackling this node.
    /// </summary>
    /// <param name="tackler">The node that started tackling this node.</param>
    /// <param name="tacklerCount">
    /// The number of nodes currently tackling this node.
    /// </param>
    public event Action<TackledEventArgs>? GotTackled;
    /// <summary>
    /// Occurs when another node stops tackling this node.
    /// </summary>
    /// <param name="tackler">The node that stopped tackling this node.</param>
    /// <param name="tacklerCount">
    /// The number of nodes currently tackling this node.
    /// </param>
    public event Action<TackledEventArgs>? GotReleased;

    /// <summary>
    /// Tries to start tackling the specified <paramref name="target"/>
    /// with the given <paramref name="limit"/>.
    /// </summary>
    /// <param name="target">The node to tackle.</param>
    /// <param name="limit">The limit imposed on <paramref name="target"/> by this node.</param>
    /// <returns>
    /// <see langword="true"/> if the tackle was established; otherwise,
    /// <see langword="false"/> if this node is already tackling another node
    /// or <paramref name="target"/> is this node.
    /// </returns>
    /// <remarks>
    /// A node can tackle at most one other node at a time.
    /// Call <see cref="Release"/> before starting a tackle on a different node.
    /// </remarks>
    public bool Tackle(TackleNode target, float limit)
    {
        if (Tackled is not null)
            return false;

        if (target == this)
            return false;

        InternalLimit = limit;
        Tackled = target;
        
        TackleStarted?.Invoke(target);
        target.AddParent(this);

        for (var n = target; n != null && !n._cycling; n = n.Tackled)
            if (n == this) { SetCycling(); break; }
        
        if (_cycling)
            return true;

        EffectiveLimit = GetEffectiveLimit();
        target.UpdateFromTackle(EffectiveLimit);

        return true;
    }

    /// <summary>
    /// Releases the node currently tackled by this node.
    /// </summary>
    /// <returns>
    /// <see langword="true"/> if a tackle was released; otherwise,
    /// <see langword="false"/> if this node was not tackling another node.
    /// </returns>
    /// <remarks>
    /// Releasing a tackle updates the limits propagated through the affected
    /// part of the graph.
    /// </remarks>
    public bool Release()
    {
        if (Tackled is null)
            return false;

        float prevEffectiveLimit = EffectiveLimit;
        InternalLimit = float.PositiveInfinity;

        var prevTackled = Tackled;
        
        TackleReleased?.Invoke(prevTackled);
        prevTackled.RemoveParent(this);

        if (_cycling)
            UnsetCycling();
        else
        {
            EffectiveLimit = GetEffectiveLimit();
            prevTackled.UpdateFromRelease(prevEffectiveLimit);
        }

        Tackled = null;
        
        return true;
    }

    /// <summary>
    /// Changes this node's internal limit.
    /// </summary>
    /// <param name="limit">The new limit.</param>
    /// <returns>
    /// <see langword="true"/> if this node is currently tackling another node and the limit was updated;
    /// otherwise, <see langword="false"/>.
    /// </returns>
    public bool SetLimit(float limit)
    {            
        if (Tackled is null)
            return false;

        float prevEffectiveLimit = EffectiveLimit;
        InternalLimit = limit;

        if (_cycling)
            return true;

        EffectiveLimit = GetEffectiveLimit();

        if (prevEffectiveLimit == EffectiveLimit)
            return true;

        // To not confuse - it is not a true release/tackle, but same optimization applies.
        if (prevEffectiveLimit < EffectiveLimit)
            Tackled.UpdateFromRelease(prevEffectiveLimit);
        else
            Tackled.UpdateFromTackle(EffectiveLimit);

        return true;
    }

    public int DodgeTacklers()
    {
        int count = _tacklers.Count;
        if (count == 0)
            return 0;

        float prevEffectiveLimit = EffectiveLimit;
        bool wasCycling = _cycling;

        var tacklers = ArrayPool<TackleNode>.Shared.Rent(count);
        _tacklers.CopyTo(tacklers);
        _tacklers.Clear();

        try
        {
            for (int i = 0; i < count; i++)
            {
                var t = tacklers[i];
                t.InternalLimit = float.PositiveInfinity;

                if (t._cycling)
                    t.UnsetCycling();
                else
                    t.EffectiveLimit = t.GetEffectiveLimit();

                t.Tackled = null;
            }

            if (!wasCycling)
            {
                _parentsLimit = float.PositiveInfinity;
                EffectiveLimit = InternalLimit;

                if (prevEffectiveLimit < EffectiveLimit)
                    Tackled?.UpdateFromRelease(prevEffectiveLimit);
            }

            for (int i = 0; i < count; i++)
            {
                tacklers[i].TackleReleased?.Invoke(this);
                GotReleased?.Invoke(new TackledEventArgs(tacklers[i], count - 1 - i));
            }
        }
        finally
        {
            ArrayPool<TackleNode>.Shared.Return(tacklers, clearArray: true);
        }

        return count;
    }
}