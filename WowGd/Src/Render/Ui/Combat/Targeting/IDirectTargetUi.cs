using System;
using WowGd.Src.Entities;

namespace WowGd.Src.Render.Ui.Combat.Targeting;

/// <summary>
/// A UI contract that defines something that can be used to display direct targeting helper.
/// 
/// For example, it could be used to assign an input to each visible entity. 
/// </summary>
public interface IDirectTargetUi
{
    /// <summary>
    /// Should be emitted when it enters screen.
    /// </summary>
    event Action<IDirectTargetUi>? ScreenEntered;
    /// <summary>
    /// Should be emitted when it exits screen.
    /// </summary>
    event Action<IDirectTargetUi>? ScreenExited;
    /// <summary>
    /// Called by the manager when it should be displayed.
    /// </summary>
    /// <returns>Whether this targeting ui could be enabled.</returns>
    bool Enable();
    /// <summary>
    /// Updates the target index attributed to this targeting ui's entity.
    /// </summary>
    /// <param name="index"></param>
    void UpdateIndex(int index);
    /// <summary>
    /// Updates the validity of this targeting ui's entity.
    /// </summary>
    /// <param name="valid"></param>
    void UpdateValidity(bool valid);
    /// <summary>
    /// Called by the manager when this targeting ui is no longer to be displayed.
    /// </summary>
    void Disable();
    /// <summary>
    /// This targeting ui's entity.
    /// </summary>
    IEntity Entity { get; }
}