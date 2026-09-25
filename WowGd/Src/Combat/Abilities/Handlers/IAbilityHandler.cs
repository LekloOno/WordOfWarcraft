namespace WowGd.Src.Combat.Abilities.Handlers;

public interface IAbilityHandler
{
    void OnStarted();
    void OnStopped();
    void OnCancelled();

    void OnTargetingStarted();
    void OnTargetingCompleted();
}