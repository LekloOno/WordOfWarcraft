namespace WowGd.Src.Combat.Abilities;

public interface IAbilityInnerCycleHandler
{
    void OnTargetingStarted();
    void OnTargetingCompleted();
    void OnActuationStarted();
    void OnActuationCompleted();
}
