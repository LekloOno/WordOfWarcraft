using Godot;
using WowGd.Src.Input.Generators;
using WowGd.Src.Tools;

namespace WowGd.Src.Input.Targeting.Free.Control;

[GlobalClass]
public partial class StandardKeyCursorMover : CursorMover<StandardKeyGenerator>
{
    [Export] private float _cursorSpeed;

    public override void _PhysicsProcess(double delta)
    {
        if (_generator.Retrieve(out Vector2 wishDir))
            _cursor.Position += wishDir * _cursorSpeed * (float) delta;
    }
}