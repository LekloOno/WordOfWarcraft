using WowGd.Src.Physics.Movement.Channels;

namespace WowGd.Src.Physics.Movement.Status;

public static class MovementStatusExt
{
    public static bool Immobilized(this MovementStatus status) =>
        status.HasFlag(MovementStatus.Immobilized);
    public static bool Stabilized(this MovementStatus status) =>
        status.HasFlag(MovementStatus.Stabilized);
    public static bool Unwarped(this MovementStatus status) =>
        status.HasFlag(MovementStatus.Unwarped);
    public static bool Airborne(this MovementStatus status, bool strict = true)
    {
        if (strict)
            return status.HasFlag(MovementStatus.Airborne)
                && !status.HasFlag(MovementStatus.Anchored);

        return status.HasFlag(MovementStatus.Airborne);
    }
    public static bool Anchored(this MovementStatus status) =>
        status.HasFlag(MovementStatus.Anchored);

    public static MovementChannels ToMovementChannels(this MovementStatus status)
    {
        MovementChannels channels = 0;

        if (status.Immobilized())
            channels |= MovementChannels.Internal;

        if (status.Stabilized())
            channels |= MovementChannels.External;

        if (status.Unwarped())
            channels |= MovementChannels.Warp;

        return channels;
    }
}