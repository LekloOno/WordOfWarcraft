namespace WowGd.Src.Combat.Abilities;

public static class AbilityBinderExt
{
    public static void Bind(this IAbilityHandler handler, IListenableAbility ability)
    {
        (handler as IAbilityLifeCycleHandler).Bind(ability);
        (handler as IAbilityInnerCycleHandler).Bind(ability);
        (handler as IAbilityLaunchesHandler).Bind(ability);
    }

    public static void Unbind(this IAbilityHandler handler, IListenableAbility ability)
    {
        (handler as IAbilityLifeCycleHandler).Unbind(ability);
        (handler as IAbilityInnerCycleHandler).Unbind(ability);
        (handler as IAbilityLaunchesHandler).Unbind(ability);
    }

    public static void Bind(this IAbilityLifeCycleHandler handler, IListenableAbility ability)
    {
        ability.Started     += handler.OnStarted;
        ability.Stopped     += handler.OnStopped;
        ability.Cancelled   += handler.OnCancelled;
    }

    public static void Unbind(this IAbilityLifeCycleHandler handler, IListenableAbility ability)
    {
        ability.Started     -= handler.OnStarted;
        ability.Stopped     -= handler.OnStopped;
        ability.Cancelled   -= handler.OnCancelled;
    }

    public static void Bind(this IAbilityInnerCycleHandler handler, IListenableAbility ability)
    {
        ability.TargetingStarted    += handler.OnTargetingStarted;
        ability.TargetingCompleted  += handler.OnTargetingCompleted;
        ability.ActuationStarted    += handler.OnActuationStarted;
        ability.ActuationCompleted  += handler.OnActuationCompleted;
    }

    public static void Unbind(this IAbilityInnerCycleHandler handler, IListenableAbility ability)
    {
        ability.TargetingStarted    -= handler.OnTargetingStarted;
        ability.TargetingCompleted  -= handler.OnTargetingCompleted;
        ability.ActuationStarted    -= handler.OnActuationStarted;
        ability.ActuationCompleted  -= handler.OnActuationCompleted;
    }

    public static void Bind(this IAbilityLaunchesHandler handler, IListenableAbility ability)
    {
        ability.MainLaunchesEmitted      += handler.OnMainLaunchesEmitted;
        ability.InstantLaunchesEmitted   += handler.OnInstantLaunchesEmitted;
        ability.TargetingLaunchesEmitted += handler.OnTargetingLaunchesEmitted;
        ability.ActuationLaunchesEmitted += handler.OnActuationLaunchesEmitted;
    }

    public static void Unbind(this IAbilityLaunchesHandler handler, IListenableAbility ability)
    {
        ability.MainLaunchesEmitted      -= handler.OnMainLaunchesEmitted;
        ability.InstantLaunchesEmitted   -= handler.OnInstantLaunchesEmitted;
        ability.TargetingLaunchesEmitted -= handler.OnTargetingLaunchesEmitted;
        ability.ActuationLaunchesEmitted -= handler.OnActuationLaunchesEmitted;
    }
}
