namespace WowGd.Src.Entities.Health;

public interface IEntityHealthHandler
{
    void OnDied();
    void OnDamaged(int hp);
    void OnHealed(int hp);
    void OnRessurected(int hp);
}