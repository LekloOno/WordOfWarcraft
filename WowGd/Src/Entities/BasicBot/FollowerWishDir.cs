using Godot;
using WowGd.Src.Physics.Movement.WishDir;
using WowGd.Src.Tools;

namespace WowGd.Src.Entities.BasicBot;

[GlobalClass]
public partial class FollowerWishDir : Node, IWishDir
{
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

        return (target.Body.GlobalPosition - _entity.Body.GlobalPosition).Normalized();
    }
}