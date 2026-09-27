using System;

namespace WowGd.Src.Physics.Movement.Status;

[Flags]
public enum MovementStatus
{
    /// <summary>
    /// Hinders internal movements.
    /// </summary>
    Immobilized = 1 << 0,
    /// <summary>
    /// Hinders external movements.
    /// </summary>
    Stabilized  = 1 << 1,
    /// <summary>
    /// Hinders warp movements.
    /// </summary>
    Unwarped    = 1 << 2,
    /// <summary>
    /// Switches internal movement handler.
    /// </summary>
    Airborne    = 1 << 3,
    /// <summary>
    /// Overrides Airborne.
    /// </summary>
    Anchored    = 1 << 4,
}