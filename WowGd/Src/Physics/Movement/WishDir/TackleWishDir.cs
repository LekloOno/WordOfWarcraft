using Godot;
using WowGd.Src.Entities;

namespace WowGd.Src.Physics.Movement.WishDir;

public class TackleWishDir(IEntity entity, IEntity target, Vector2 offset) : IWishDir
{
    public bool Enabled => _enabled;
    private bool _enabled = true;

    private readonly IEntity _entity = entity;
    private readonly IEntity _target = target;
    private readonly Vector2 _offset = offset;

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

        Vector2 dir = TargetPosition - _entity.Body.GlobalPosition;
        return dir.Normalized();
    }

    private Vector2 TargetPosition => _target.Body.GlobalPosition + _offset;
}