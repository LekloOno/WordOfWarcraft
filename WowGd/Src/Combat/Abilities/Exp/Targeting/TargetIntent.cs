using Godot;
using WowGd.Src.Entities;

namespace WowGd.Src.Combat.Abilities.Exp.Targeting;

public readonly struct TargetIntent
{
    public TargetIntent(IEntity entity) : this()
    {
        Entity      = entity;
        _position   = default;
        IsDirect    = true;
    }

    public TargetIntent(Vector2 position) : this()
    {
        Entity      = null;
        _position   = position;
        IsDirect    = false;
    }

    public readonly IEntity? Entity;
    private readonly Vector2 _position;
    public readonly bool IsDirect;

    public Vector2 Position =>
        IsDirect
        ? Entity!.Body.GlobalPosition
        : _position;
}