using WowGd.Src.Combat.Resources.FocusRes;

namespace WowGd.Src.Combat.Resources;

public interface IResourceManager
{
    IFocus? Focus { get; }
}