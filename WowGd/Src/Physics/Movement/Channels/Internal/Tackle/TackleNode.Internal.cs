using System;

namespace WowGd.Src.Physics.Movement.Channels.Internal.Tackle;

public sealed partial class TackleNode
{
    private void RemoveParent(TackleNode parent)
    {
        if (_tacklers.SwapRemove(parent))
            GotReleased?.Invoke(new TackledEventArgs(parent, _tacklers.Count));
    }

    private void AddParent(TackleNode parent)
    {
        _tacklers.Add(parent);
        GotTackled?.Invoke(new TackledEventArgs(parent, _tacklers.Count));
    }

    /// <summary>
    /// Non-recursive computation of parents limit, using parents caches.
    /// </summary>
    /// <returns></returns>
    private float GetParentsLimit()
    {
        float parentsLimit = float.PositiveInfinity;
        foreach (TackleNode tackler in _tacklers)
            if (tackler.EffectiveLimit < parentsLimit)
                parentsLimit = tackler.EffectiveLimit;

        return parentsLimit;
    }

    /// <summary>
    /// Retrieves the current EffectiveLimit using own and parents caches.
    /// </summary>
    /// <returns></returns>
    private float GetEffectiveLimit() =>
        MathF.Min(InternalLimit, _parentsLimit);

    /// <summary>
    /// Avoids retraversing the parents when we know it's a new limit.
    /// We can simply compare it to existing cached limits.
    /// </summary>
    /// <param name="parentEffectiveLimit"></param>
    private void UpdateFromTackle(float parentEffectiveLimit)
    {
        if (_cycling) return;
            
        // This parent is not more limiting than an existing parent.
        // So we can avoid any further computation.
        if (parentEffectiveLimit >= _parentsLimit) return;

        // Otherwise, well, we already know the new most limiting..
        _parentsLimit = parentEffectiveLimit;

        // Yet, if this is still less limiting than the own limit,
        // This was not the most limiting factor, own was.
        // We thus don't need to propagate.
        if (parentEffectiveLimit >= InternalLimit) return;

        EffectiveLimit = parentEffectiveLimit;
        Tackled?.UpdateFromTackle(parentEffectiveLimit);
    }

    private void UpdateFromRelease(float parentEffectiveLimit)
    {
        if (_cycling) return;

        // This parent was not the most limiting parent.
        // So we can avoid any further computation.
        if (parentEffectiveLimit > _parentsLimit) return;
        
        // Otherwise, we can't figure out the new most limiting
        // factor unless we re-traverse the whole parents.
        float newLimit = GetParentsLimit();
        // No need to propagate if a remaining parent has the same limit.
        if (newLimit == _parentsLimit) return;

        _parentsLimit = newLimit;

        // Yet, if this parent was less limiting than the own limit,
        // This was not the most limiting factor, own was.
        // We thus don't need to propagate.
        if (parentEffectiveLimit >= InternalLimit) return;

        EffectiveLimit = GetEffectiveLimit();
        Tackled?.UpdateFromRelease(parentEffectiveLimit);
    }

    private void SetCycling()
    {
        var n = this;
        do
        {
            n._cycling = true;
            n._parentsLimit = n.EffectiveLimit = 0f;
            n = n.Tackled!;
        } while (n != this);
    }

    private void UnsetCycling()
    {
        var n = this;
        do
        {
            n._cycling = false;
            n = n.Tackled!;
            n._parentsLimit = n.GetParentsLimit();
            n.EffectiveLimit = n.GetEffectiveLimit();
        } while (n != this);
    }
}