using WowGd.Src.Combat.Abilities.Behaviors;
using WowGd.Src.Combat.Abilities.CoolDowns;

namespace WowGd.Src.Combat.Abilities.Handlers;

public static class AbilityBinderExt
{
    public static void TryBindAll(this IAbilityHandler handler, IAbility ability)
    {
        handler.Bind(ability);

        if (handler is ICoolDownHandler cdHandler &&
            ability is ICoolDownable cdAbility)
            cdHandler.Bind(cdAbility.CoolDown);
        if (handler is IActuableHandler actHandler &&
            ability is IActuableAbility actAbility)
            actHandler.Bind(actAbility);
        if (handler is ICancelLaunchableHandler cancelHandler &&
            ability is ICancelLaunchable cancelAbility)
            cancelHandler.Bind(cancelAbility);
        if (handler is IStopLaunchableHandler stopHandler &&
            ability is IStopLaunchable stopAbility)
            stopHandler.Bind(stopAbility);
        if (handler is ITargetLaunchableHandler targetHandler &&
            ability is ITargetLaunchable targetAbility)
            targetHandler.Bind(targetAbility);
    }

    public static void TryUnbindAll(this IAbilityHandler handler, IAbility ability)
    {
        handler.Unbind(ability);

        if (handler is ICoolDownHandler cdHandler &&
            ability is ICoolDownable cdAbility)
            cdHandler.Unbind(cdAbility.CoolDown);
        if (handler is IActuableHandler actHandler &&
            ability is IActuableAbility actAbility)
            actHandler.Unbind(actAbility);
        if (handler is ICancelLaunchableHandler cancelHandler &&
            ability is ICancelLaunchable cancelAbility)
            cancelHandler.Unbind(cancelAbility);
        if (handler is IStopLaunchableHandler stopHandler &&
            ability is IStopLaunchable stopAbility)
            stopHandler.Unbind(stopAbility);
        if (handler is ITargetLaunchableHandler targetHandler &&
            ability is ITargetLaunchable targetAbility)
            targetHandler.Unbind(targetAbility);
    }

    public static void Bind(this IAbilityHandler handler, IAbility ability)
    {
        ability.Started             += handler.OnStarted;
        ability.Stopped             += handler.OnStopped;
        ability.Cancelled           += handler.OnCancelled;
        ability.TargetingStarted    += handler.OnTargetingStarted;
        ability.TargetingCompleted  += handler.OnTargetingCompleted;
        ability.TargetingFailed     += handler.OnTargetingFailed;
    }

    public static void Unbind(this IAbilityHandler handler, IAbility ability)
    {
        ability.Started             -= handler.OnStarted;
        ability.Stopped             -= handler.OnStopped;
        ability.Cancelled           -= handler.OnCancelled;
        ability.TargetingStarted    -= handler.OnTargetingStarted;
        ability.TargetingCompleted  -= handler.OnTargetingCompleted;
    }

    public static void Bind(this ICoolDownHandler handler, ICoolDown ability)
    {
        ability.CdStartedAt     += handler.OnCdStartedAt;
        ability.CdReduced       += handler.OnCdReduced;
        ability.CdEnlenghted    += handler.OnCdEnlengthed;
        ability.CdCompleted     += handler.OnCdCompleted;
    }

    public static void Unbind(this ICoolDownHandler handler, ICoolDown ability)
    {
        ability.CdStartedAt     -= handler.OnCdStartedAt;
        ability.CdReduced       -= handler.OnCdReduced;
        ability.CdEnlenghted    -= handler.OnCdEnlengthed;
        ability.CdCompleted     -= handler.OnCdCompleted;
    }

    public static void Bind(this IActuableHandler handler, IActuableAbility ability)
    {
        ability.ActuationLaunchesEmitted    += handler.OnActuationLaunchesEmitted;
        ability.ActuationCompleted          += handler.OnActuationCompleted;
        ability.ActuationStarted            += handler.OnActuationStarted;
    }

    public static void Unbind(this IActuableHandler handler, IActuableAbility ability)
    {
        ability.ActuationLaunchesEmitted    -= handler.OnActuationLaunchesEmitted;
        ability.ActuationCompleted          -= handler.OnActuationCompleted;
        ability.ActuationStarted            -= handler.OnActuationStarted;
    }

    public static void Bind(this ICancelLaunchableHandler handler, ICancelLaunchable ability)
    {
        ability.CancelLaunchesEmitted   += handler.OnCancelLaunchesEmitted;
    }

    public static void Unbind(this ICancelLaunchableHandler handler, ICancelLaunchable ability)
    {
        ability.CancelLaunchesEmitted   -= handler.OnCancelLaunchesEmitted;
    }

    public static void Bind(this IStopLaunchableHandler handler, IStopLaunchable ability)
    {
        ability.StopLaunchesEmitted   += handler.OnStopLaunchesEmitted;
    }

    public static void Unbind(this IStopLaunchableHandler handler, IStopLaunchable ability)
    {
        ability.StopLaunchesEmitted   -= handler.OnStopLaunchesEmitted;
    }

    public static void Bind(this ITargetLaunchableHandler handler, ITargetLaunchable ability)
    {
        ability.TargetLaunchesEmitted   += handler.OnTargetLaunchesEmitted;
    }

    public static void Unbind(this ITargetLaunchableHandler handler, ITargetLaunchable ability)
    {
        ability.TargetLaunchesEmitted   -= handler.OnTargetLaunchesEmitted;
    }
}
