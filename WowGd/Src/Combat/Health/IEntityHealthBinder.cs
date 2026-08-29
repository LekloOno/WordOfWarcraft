namespace WowGd.Src.Combat.Health;

public interface IEntityHealthBinder
{
    void Bind(IEntityHealth health);
    void Unbind(IEntityHealth health);
}