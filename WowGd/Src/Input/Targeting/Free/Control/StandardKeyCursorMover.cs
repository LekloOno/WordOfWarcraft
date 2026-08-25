using Godot;
using WowGd.Src.Input.Generators;

namespace WowGd.Src.Input.Targeting.Free.Control;

[GlobalClass]
public partial class StandardKeyCursorMover : CursorMover<StandardKeyGenerator>
{
    public StandardKeyCursorMover() {}
    public StandardKeyCursorMover(Cursor cursor) : base(cursor) {}

    [Export] private float _cursorSpeed = 15f;

    public override void _PhysicsProcess(double delta)
    {
        if (_generator.Retrieve(out Vector2 wishDir))
            _cursor.Position += wishDir * _cursorSpeed * (float) delta;
    }
}