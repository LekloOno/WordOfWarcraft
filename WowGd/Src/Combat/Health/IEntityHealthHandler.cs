using WowGd.Src.Combat.Resources;

namespace WowGd.Src.Combat.Health;

public interface IEntityHealthHandler : IStandardResourceHandler
{
    void OnDied();
    void OnResurrected(int hp);
}