namespace WowGd.Src.Tools;

/// <summary>
/// Extension of IDisablable that can be monitored before taking actions.
/// </summary>
public interface IMoniDisablable : IDisablable
{
    /// <summary>
    /// The contracts is that if this returns true in a given cycle, then Disable should too.
    /// </summary>
    /// <returns></returns>
    bool CanDisable();
    /// <summary>
    /// The contracts is that if this returns true in a given cycle, then Enable should too.
    /// </summary>
    /// <returns></returns>
    bool CanEnable();
}