using Godot;
using WowGd.Src.Input.Hands;
using WowGd.Src.Tools;

namespace WowGd.Src.Physics.Movement.WishDir;

[GlobalClass]
public partial class WishDirFirstHandMode : Node, IFirstHandInputMode
{
    private IWishDir _wishDir = null!;

    public WishDirFirstHandMode() { }
    public WishDirFirstHandMode(IWishDir wishDir) { _wishDir = wishDir; }

    public override void _Ready()
    {
        if (_wishDir == null && this.TryGetComposed(out IWishDir? wishDir))
            _wishDir = wishDir;

        HandsInputManager.TryPushFirstHandMode(this);
    }

    public bool CanStart() => true;
    public void Start() => _wishDir.Enable();
    public bool CanStop() => true;
    public void Stop() => _wishDir.Disable();
}