using Godot;
using WowGd.Src.Input.Generators;

namespace WowGd.Src.Input.Targeting.Free.Control;

[GlobalClass]
public partial class RelativeKeyCursorMover : CursorMover<RelativeKeyGenerator>
{
    public override void _PhysicsProcess(double delta)
    {
        if (_generator.Retrieve(out Vector2 translation))
            _cursor.Position += translation;
    }
}