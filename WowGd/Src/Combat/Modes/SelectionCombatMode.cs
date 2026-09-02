using System.Collections.Generic;
using Godot;
using WowGd.Src.Combat.Abilities;
using WowGd.Src.Entities;
using WowGd.Src.Tools;

namespace WowGd.Src.Combat.Modes;

/// <summary>
/// A mode where you can freely select abilities, like in a standard RPG.
/// 
/// Abilities themselves might however be tied to specific trigger behaviors, like dactylography actions.
/// </summary>
[GlobalClass]
public partial class SelectionCombatMode : CombatMode
{
    private IEntity _entity = null!;
    private readonly List<IAbility> _abilities = [];
    private readonly List<IAbility> _activeAbilities = [];
    protected override bool PreReadySpec()
    {
        this.GetComponents(_abilities);

        if (this.TryGetComposedRecursive(out IEntity? entity))
            _entity = entity;


        //foreach (IAbility ability in _abilities)
        //    ability.Stopped += () => OnAbilityStop(ability);

        // even if there's 0 abilities, it's not a functionnal problem.
        return true;
    }

    private void OnAbilityStop(IAbility ability)
    {
        _activeAbilities.Remove(ability);
    }

    protected override void ActivateSpec() {}

    protected override void DeactivateSpec()
    {
        foreach (IAbility ability in _activeAbilities)
            ability.Cancel(_entity);

        _activeAbilities.Clear();
    }

    protected override void DisableSpec()
    {
        foreach (IAbility ability in _activeAbilities)
            ability.Disable();
    }

    protected override void EnableSpec()
    {
        foreach (IAbility ability in _activeAbilities)
            ability.Enable();
    }

    public bool Select(int index)
    {
        if (!Active)
            return false;
        
        if (index >= _abilities.Count)
            return false;

        IAbility ability = _abilities[index];
        _activeAbilities.Add(ability);
        ability.Start(_entity);
        return true;
    }

    public bool HasActive() =>
        _activeAbilities.Count != 0;

    public bool Unselect(int index)
    {
        if (!Active)
            return false;

        if (index >= _activeAbilities.Count)
            return false;

        if (!_activeAbilities[index].Cancel(_entity))
            return false;

        _activeAbilities.RemoveAt(index);
        return true;
    }
}