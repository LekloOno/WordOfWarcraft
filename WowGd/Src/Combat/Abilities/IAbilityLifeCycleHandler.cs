namespace WowGd.Src.Combat.Abilities;

public interface IAbilityLifeCycleHandler
{
    void OnStarted();
    void OnStopped();
    void OnCancelled();
    void OnCoolDownStarted(ulong cooldown);
    void OnCoolDownCancelled();
}