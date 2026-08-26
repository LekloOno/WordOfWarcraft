using Godot;
using WowGd.Src.Physics;

namespace WowGd.Src.Render.Animation.Entity;

[GlobalClass]
public partial class EntityAnimator : Node
{
    [Export]
    private EntityRender3D _render = null!;

    public override void _Ready()
    {

    }
}
