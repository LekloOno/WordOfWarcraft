using System;

namespace WowGd.Src.Tools;

public class ProcessToggle : IDisablable
{
    public bool Enabled { get; private set; } = false;
    public event Action<bool>? Toggled;

    public bool Enable()
    {
        if (Enabled)
            return false;

        Enabled = true;
        Toggled?.Invoke(Enabled);
        return true;
    }

    public bool Disable()
    {
        if (!Enabled)
            return false;

        Enabled = false;
        Toggled?.Invoke(Enabled);
        return true;
    }
}