namespace WowGd.Src.Combat.Health;

public interface IEntityHealthHandler
{
    void OnDied();
    void OnDamaged(int hp);
    void OnHealed(int hp);
    void OnResurrected(int hp);
}