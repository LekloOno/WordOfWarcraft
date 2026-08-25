using WowGd.Src.Tools;

namespace WowGd.Src.Input;

public interface IMode : IDisablable
{
    bool Activate();
    bool Deactivate();
}