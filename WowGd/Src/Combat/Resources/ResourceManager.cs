using Godot;
using WowGd.Src.Combat.Resources.FocusRes;
using WowGd.Src.Tools;

namespace WowGd.Src.Combat.Resources;

[GlobalClass]
public partial class ResourceManager : Node, IResourceManager
{
    public IFocus? Focus { get; private set; }

    public override void _Ready()
    {
        if (this.TryGetComponent(out IFocus? focus))
            Focus = focus;
    }
}