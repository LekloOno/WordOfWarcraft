using WowGd.Src.Combat.Abilities.Targeting;

namespace WowGd.Src.Combat.Abilities.Handlers;

public interface IAbilityHandler
{
    void OnStarted();
    void OnStopped();
    void OnCancelled();

    void OnTargetingStarted();
    void OnTargetingCompleted();
    void OnTargetingFailed(TargetFailure failure);
}