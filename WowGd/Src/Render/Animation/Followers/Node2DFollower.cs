using Godot;
using WowGd.Src.Tools;

namespace WowGd.Src.Render.Animation.Followers;

[GlobalClass]
public abstract partial class Node2DFollower : Node2D
{
    [Export] public Node2D Node = null!;

    public override sealed void _Ready()
    {
        TopLevel = true;

        if (!TryFetchNode())
            return;

        ReadySpec();
    }

    private bool TryFetchNode()
    {
        if (Node != null)
            return true;

        if (this.TryGetComposed(out Node2D? parent))
        {
            Node = parent;
            return true;
        }

        if (this.TryGetComponent(out Node2D? child))
        {
            Node = child;
            return true;
        }

        return false;
    }

    /// <summary>
    /// Called after base Ready procedure to extend setup behavior.
    /// </summary>
    protected virtual void ReadySpec() {}
}