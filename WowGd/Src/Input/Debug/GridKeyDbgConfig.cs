using Godot;
using WowGd.Src.Input.Targeting.Free;

namespace WowGd.Src.Input.Debug;

[GlobalClass]
public partial class GridKeyDbgConfig : Node
{
    public override void _EnterTree()
    {
        GridKey.Register(Key.Q,         new(-4, 0));
        GridKey.Register(Key.S,         new(-3, 0));
        GridKey.Register(Key.D,         new(-2, 0));
        GridKey.Register(Key.F,         new(-1, 0));

        GridKey.Register(Key.J,         new(1, 0));
        GridKey.Register(Key.K,         new(2, 0));
        GridKey.Register(Key.L,         new(3, 0));
        GridKey.Register(Key.M,         new(4, 0));

        GridKey.Register(Key.Key5,      new(0, -2));
        GridKey.Register(Key.T,         new(0, -1));
        GridKey.Register(Key.B,         new(0, 1));

        GridKey.Register(Key.Key4,      new(-1, -2));
        GridKey.Register(Key.R,         new(-1, -1));
        GridKey.Register(Key.V,         new(-1, 1));

        GridKey.Register(Key.Key3,      new(-2, -2));
        GridKey.Register(Key.E,         new(-2, -1));
        GridKey.Register(Key.C,         new(-2, 1));

        GridKey.Register(Key.Z,         new(-3, -1));
        GridKey.Register(Key.X,         new(-3, 1));

        GridKey.Register(Key.Key7,      new(1, -2));
        GridKey.Register(Key.U,         new(1, -1));
        GridKey.Register(Key.Comma,     new(1, 1));

        GridKey.Register(Key.Key8,      new(2, -2));
        GridKey.Register(Key.I,         new(2, -1));
        GridKey.Register(Key.Semicolon, new(2, 1));

        GridKey.Register(Key.O,         new(3, -1));
        GridKey.Register(Key.Colon,     new(3, 1));
    }
}