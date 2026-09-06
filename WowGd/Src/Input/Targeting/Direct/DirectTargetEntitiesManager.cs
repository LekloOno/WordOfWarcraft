using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Godot;
using WowGd.Src.Combat.Abilities.Data;
using WowGd.Src.Combat.Abilities.Targeting.TargetRules;
using WowGd.Src.Entities;
using WowGd.Src.Render.Ui.Combat.Targeting;

namespace WowGd.Src.Input.Targeting.Direct;

/// <summary>
/// Auto load node to handle attribution of targeting index to visible entities.
/// </summary>
public partial class DirectTargetEntitiesManager : Node
{
    private static DirectTargetEntitiesManager Instance = null!;

    private static readonly List<IDirectTargetUi> _availableTargets = [];
    private static readonly List<IDirectTargetUi> _activeTargets = [];

    private static bool _enabled = false;
    private static IEnumerable<ITargetRule> _targetRules = [];
    private static IEntity? _caller;

    public override void _Ready()
    {
        Instance = this;
    }

    public static void Register(IDirectTargetUi directTargetUi3D)
    {
        directTargetUi3D.ScreenEntered += OnScreenEntered;
        directTargetUi3D.ScreenExited  += OnScreenExited;
    }

    public static void Unregister(IDirectTargetUi directTargetUi3D)
    {
        directTargetUi3D.ScreenEntered -= OnScreenEntered;
        directTargetUi3D.ScreenExited  -= OnScreenExited;
    }

    private static void OnScreenEntered(IDirectTargetUi ui)
    {
        if (!_enabled || !TryAddEnable(ui))
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

        return true;
    }

    public static bool Enable(IEntity caller, IEnumerable<ITargetRule>? rules)
    {
        if (_enabled)
            return false;
        
        _enabled = true;
        
        _caller = caller;
        _targetRules = rules ?? [];

        _availableTargets.RemoveAll(TryAddEnable);
        
        _activeTargets.Sort((a, b) =>
            a.Entity.DistanceSquaredTo(caller)
            .CompareTo(b.Entity.DistanceSquaredTo(caller)));
        
        for (int i = 0; i < _activeTargets.Count; i ++)
            _activeTargets[i].UpdateIndex(i);

        Instance.SetPhysicsProcess(true);

        return true;
    }

    private static bool TryDisable(IDirectTargetUi ui)
    {
        if (!_activeTargets.Remove(ui))
            return false;
        
        ui.Disable();
        return true;
    }

    private static bool TryAddEnable(IDirectTargetUi ui)
    {
        // Possibly some more logic later, typically, maximum distance ?
        if (_activeTargets.Count > AbilitySelectInputExt.MaxIndex)
            return false;

        return TryFinalizeEnable(ui);
    }

    /// <summary>
    /// Short circuit if called directly -
    /// Must not be called if the target is still in _availableTargets
    /// </summary>
    /// <param name="ui"></param>
    private static bool TryFinalizeEnable(IDirectTargetUi ui)
    {
        if (!ui.Enable())
            return false;

        int index = _activeTargets.Count;
        _activeTargets.Add(ui);
        ui.UpdateIndex(index);
        ui.UpdateValidity(GetTargetValidity(ui));
        return true;
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

    private static IDirectTargetUi? _selected;
    public static bool TryRetrieveEntity(int index, [NotNullWhen(true)] out IEntity? entity)
    {
        if (_activeTargets.Count <= index)
        {
            entity = null;
            return false;
        }

        _selected?.Unselect();
        _selected = _activeTargets[index];
        _selected.Select();

        entity = _selected.Entity;
        return true;
    }

    public static bool TryRetrieveClosestEntity(IEntity caster, [NotNullWhen(true)] out IEntity? entity, IEnumerable<ITargetRule>? targetRules = null)
    {
        float best = float.PositiveInfinity;

        entity = null;
        targetRules ??= [];
        
        IDirectTargetUi? selected = null;

        foreach (IDirectTargetUi ui in _availableTargets)
        {
            if (caster == ui.Entity)
                continue;

            if (!targetRules.CheckAll(caster, new(ui.Entity)))
                continue;

            float squaredDist = caster.DistanceSquaredTo(ui.Entity);
            if (squaredDist < best)
            {
                selected = ui;
                best = squaredDist;
                entity = ui.Entity;
            }
        }

        bool retrieved = entity != null;
        if (retrieved)
        {
            Unselect();
            _selected = selected;
            _selected!.Select();
        }

        return retrieved;
    }

    public static void Unselect() =>
        _selected?.Unselect();
}