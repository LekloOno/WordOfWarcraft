namespace WowGd.Src.Physics.Movement.Channels.Internal.Tackle;

public interface IDynamicTackleNodeHandler
{
    void OnTackleStarted(DynamicTackleNode tackleNode);
    void OnTackleReleased(DynamicTackleNode tackleNode);
    void OnGotTackled(DynamicTackledEventArgs tackleArgs);
    void OnGotReleased(DynamicTackledEventArgs tackleArgs);
    void OnStaminaTicked(float prev, float next);
}