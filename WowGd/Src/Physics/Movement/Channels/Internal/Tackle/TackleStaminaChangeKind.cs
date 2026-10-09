namespace WowGd.Src.Physics.Movement.Channels.Internal.Tackle;

public enum StaminaChangeKind
{
    Tick,
    Drain,
    Feed
}

public readonly record struct StaminaChange(StaminaChangeKind Kind, double Delta, double Value);