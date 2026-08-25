using Godot;
using WowGd.Src.Tools;

namespace WowGd.Src.Combat.Health.Handlers;

public abstract partial class HealthHandler<T> : Node, IEntityHealthHandler
{
    protected T _component = default!;

    public override sealed void _Ready()
    {
        if (_component == null)
            this.TryGetSiblingComponent(out _component!);

        SpecReady();
    }

    protected virtual void SpecReady() {}
    public abstract void OnDamaged(int hp);
    public abstract void OnDied();
    public abstract void OnHealed(int hp);
    public abstract void OnRessurected(int hp);
}