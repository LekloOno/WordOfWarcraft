namespace WowGd.Src.Combat.Abilities.Targeting;

public enum TargetFailure
{
    NoTarget,
    RuleViolation,
}

public readonly record struct TargetResult
{
    public TargetIntent? Intent { get; init; }
    public TargetFailure? Failure { get; init; }
    public bool IsSuccess => Intent is not null;

    public static TargetResult Ok(TargetIntent intent) =>
        new() { Intent = intent };
    public static TargetResult Fail(TargetFailure failure) =>
        new() { Failure = failure };

    public bool TryGet(out TargetIntent intent)
    {
        intent = Intent ?? default;
        return Intent.HasValue;
    }
}