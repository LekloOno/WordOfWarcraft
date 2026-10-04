using Godot;
using WowGd.Src.Entities;
using WowGd.Src.Entities.BasicBot;
using WowGd.Src.Tools;

namespace WowGd.Src.Physics.Movement.WishDir;

[GlobalClass]
public partial class FollowerWishDir : Node, IWishDir
{
    [Export] public bool _flee = false;
    public bool Enabled => _enabled;
    private bool _enabled = true;

    private ITargetAcquirer _targetAcquirer = null!;
    private IEntity _entity = null!;

    public override void _Ready()
    {
        if (!this.TryGetSiblingComponent(out ITargetAcquirer? targetAcquirer))
            return;
        
        _targetAcquirer = targetAcquirer;

        if (this.TryGetComposedRecursive(out IEntity? entity))
            _entity = entity;
    }

    public bool Disable()
    {
        _enabled = false;
        return true;
    }

    public bool Enable()
    {
        _enabled = true;
        return true;
    }

    public Vector2 WishDir()
    {
        if (!_enabled)
            return Vector2.Zero;

        if (_targetAcquirer.Target is not IEntity target)
            return Vector2.Zero;

        int sign = _flee ? -1 : 1;
        return sign * (target.Body.GlobalPosition - _entity.Body.GlobalPosition).Normalized();
    }
}