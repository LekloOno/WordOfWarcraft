using Godot;

public static class MoveModeInputExt
{
    public const string MovementAbility = "movement_ability";
    public const string Dodging         = "dodging";
    public const string Locking         = "locking";

    public static bool IsMovementAbilityPressed(this InputEvent @event) =>
        @event.IsActionPressed(MovementAbility);

    public static bool IsDodgingPressed(this InputEvent @event) =>
        @event.IsActionPressed(Dodging);

    public static bool IsLockingPressed(this InputEvent @event) =>
        @event.IsActionPressed(Locking);
}