using WowGd.Src.Physics.Movement.Channels.Internal.Tackle;

public static class DynamicTackleNodeBinderExt
{
    public static void Bind(this IDynamicTackleNodeHandler handler, DynamicTackleNode tackleNode)
    {
        tackleNode.TackleStarted    += handler.OnTackleStarted;
        tackleNode.TackleReleased   += handler.OnTackleReleased;
        tackleNode.TackleChanged    += handler.OnTackleChanged;
        tackleNode.StaminaChanged   += handler.OnStaminaChanged;
    }

    public static void Unbind(this IDynamicTackleNodeHandler handler, DynamicTackleNode tackleNode)
    {
        tackleNode.TackleStarted    -= handler.OnTackleStarted;
        tackleNode.TackleReleased   -= handler.OnTackleReleased;
        tackleNode.TackleChanged    -= handler.OnTackleChanged;
        tackleNode.StaminaChanged   -= handler.OnStaminaChanged;
    }
}