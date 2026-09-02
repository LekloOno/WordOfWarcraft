namespace WowGd.Src.Input.Hands;

public interface IHandInputMode
{
    /// <summary>
    /// Returns whether the mode can be started or is already started.
    /// </summary>
    /// <returns>Whether it can be started or is already started.</returns>
    bool CanStart();
    void Start();
    /// <summary>
    /// Returns whether the mode can be stopped or is already stopped.
    /// </summary>
    /// <returns>Whether it can be stopped or is already stopped.</returns>
    bool CanStop();
    void Stop();
}