using Godot;
using WowGd.Src.Tools;

namespace WowGd.Src.Render.Animation.Followers;

[GlobalClass]
public abstract partial class Node2DFollower : Node2D
{
    [Export] protected Node2D _node       = null!;

    public override sealed void _Ready()
    {
        TopLevel = true;

        if (!TryFetchNode())
            return;

        ReadySpec();
    }

    private bool TryFetchNode()
    {
        if (_node != null)
            return true;

        if (this.TryGetComposed(out Node2D? parent))
        {
            _node = parent;
            return true;
        }

        if (this.TryGetComponent(out Node2D? child))
        {
            _node = child;
            return true;
        }

        return false;
    }

    /// <summary>
    /// Called after base Ready procedure to extend setup behavior.
    /// </summary>
    protected virtual void ReadySpec() {}
}