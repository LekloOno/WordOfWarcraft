namespace WowGd.Src.Tools;

public interface IDisablable
{
    bool Enable();
    bool Disable();
    bool Enabled {get;}
}