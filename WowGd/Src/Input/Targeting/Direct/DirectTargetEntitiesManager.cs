using System.Collections.Generic;
using Godot;
using WowGd.Src.Combat.Abilities.Exp.Data;
using WowGd.Src.Entities;
using WowGd.Src.Render.Ui.Combat.Targeting;

namespace WowGd.Src.Input.Targeting.Direct;

/// <summary>
/// Auto load node to handle attribution of targeting index to visible entities.
/// </summary>
public partial class DirectTargetEntitiesManager : Node
{
    private static DirectTargetEntitiesManager Instance = null!;

    private static readonly HashSet<IDirectTargetUi> _availableTargets = [];
    private static readonly HashSet<IDirectTargetUi> _activeTargets = [];

    private static bool _enabled = false;
    private static int _activeCount = 0;
    private static IEnumerable<ITargetRule> _targetRules = [];
    private static IEntity? _caller;

    public override void _Ready()
    {
        Instance = this;
    }

    public static void Register(IDirectTargetUi directTargetUi3D)
    {
        directTargetUi3D.ScreenEntered += OnScreenEntered;
        directTargetUi3D.ScreenEntered += OnScreenExited;
    }

    public static void Unregister(IDirectTargetUi directTargetUi3D)
    {
        directTargetUi3D.ScreenEntered -= OnScreenEntered;
        directTargetUi3D.ScreenEntered -= OnScreenExited;
    }

    private static void OnScreenEntered(IDirectTargetUi ui)
    {
        if (_enabled)
            TryAddEnable(ui);
        else
            _availableTargets.Add(ui);
    }

    private static void OnScreenExited(IDirectTargetUi ui)
    {
        if (_enabled && TryDisable(ui))
            return;

        _availableTargets.Remove(ui);
    }

    public static bool Disable()
    {
        if (!_enabled)
            return false;

        _caller = null;
        _targetRules = [];

        Instance.SetPhysicsProcess(false);
        _enabled = false;

        foreach (IDirectTargetUi targetUi in _activeTargets)
        {
            targetUi.Disable();
            _availableTargets.Add(targetUi);
        }

        _activeTargets.Clear();
        _activeCount = 0;


        return true;
    }

    public static bool Enable(IEntity caller, IEnumerable<ITargetRule>? rules)
    {
        if (_enabled)
            return false;

        _caller = caller;
        _targetRules = rules ?? [];

        foreach (IDirectTargetUi ui in _availableTargets)
            TryEnable(ui);

        Instance.SetPhysicsProcess(true);

        return true;
    }

    private static void TryEnable(IDirectTargetUi ui)
    {
        if (TryAddEnable(ui))
            _availableTargets.Remove(ui);
    }

    private static bool TryDisable(IDirectTargetUi ui)
    {
        if (!_activeTargets.Remove(ui))
            return false;
        
        ui.Disable();
        _activeCount --;
        return true;
    }

    private static bool TryAddEnable(IDirectTargetUi ui)
    {
        // Possibly some more logic later, typically, max number of targets, maximum distance, etc.
        FinalizeEnable(ui);
        return true;
    }

    /// <summary>
    /// Short circuit if called directly -
    /// Must not be called if the target is still in _availableTargets
    /// </summary>
    /// <param name="ui"></param>
    private static void FinalizeEnable(IDirectTargetUi ui)
    {
        _activeTargets.Add(ui);
        ui.Enable();
        ui.UpdateIndex(_activeCount);
        ui.UpdateValidity(GetTargetValidity(ui));

        _activeCount ++;
    }

    private static bool GetTargetValidity(IDirectTargetUi ui)
    {
        if (_caller == null || _targetRules == null)
            return true;
        
        return _targetRules.CheckAll(_caller, new(ui.Entity));
    }

    public override void _PhysicsProcess(double delta)
    {
        foreach (IDirectTargetUi ui in _activeTargets)
            ui.UpdateValidity(GetTargetValidity(ui));
    }
}