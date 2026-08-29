using WowGd.Src.Tools;

namespace WowGd.Src.Entities.Modes;

public interface IMode : IDisablable
{
    bool Activate();
    bool Deactivate();
}