namespace WowGd.Src.Physics.Movement.Channels.Internal.Tackle;

public interface IDynamicTackleNodeHandler
{
    void OnTackleStarted(DynamicTackleNode tackleNode);
    void OnTackleReleased(DynamicTackleNode tackleNode);
    void OnTackleChanged(TackleInteraction tackleArgs);
    void OnStaminaChanged(StaminaChange change);
}