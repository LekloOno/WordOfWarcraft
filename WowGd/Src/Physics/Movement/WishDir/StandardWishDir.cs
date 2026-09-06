using Godot;
using WowGd.Src.Input.Generators;
using WowGd.Src.Input.Hands;

namespace WowGd.Src.Physics.Movement.WishDir;

[GlobalClass]
public partial class StandardWishDir : Node, IWishDir
{
    [Export] private HandsEnum _handsInputMode = HandsEnum.None;
    private StandardKeyGenerator _generator = new();

    public override void _Ready()
    {
        AddChild(_generator);
        
        if (_handsInputMode == HandsEnum.None)
            return;

        if (_handsInputMode == HandsEnum.First)
            AddChild(new WishDirFirstHandMode(this));
        else if (_handsInputMode == HandsEnum.Second)
            AddChild(new WishDirSecondHandMode(this));
        else
            GD.PushError($"[{nameof(StandardWishDir)}] can only be derived as either a first or second hand mode, not [{_handsInputMode}].");
    }

    public Vector2 WishDir()
    {
        _generator.Retrieve(out Vector2 wishDir);
        return wishDir;
    }

    public bool Disable()
    {
        return _generator.Disable();
    }

    public bool Enable()
    {
        return _generator.Enable();
    }

    public bool Enabled => _generator.Enabled;
}