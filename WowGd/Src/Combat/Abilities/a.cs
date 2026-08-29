using Godot;
using WowGd.Src.Entities;

public interface ITargetIntent
{
    IEntity? Entity { get; }
    Vector2 Position { get; }
}

public readonly struct DirectTargetIntent(IEntity entity) : ITargetIntent
{
    public readonly IEntity? Entity => entity;
    public readonly Vector2 Position => entity.Body.GlobalPosition;
}

public readonly struct IndirectTargetIntent(Vector2 position) : ITargetIntent
{
    public readonly IEntity? Entity => null;
    public readonly Vector2 Position => position;
}

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