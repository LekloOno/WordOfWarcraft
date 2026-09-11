using System.Collections.Generic;
using System.Threading.Tasks;
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
public partial class SelectionCombatMode : CombatMode, IInitializable
{
    private IEntity _entity = null!;
    private readonly List<IAbility> _abilities = [];
    private readonly List<IAbility> _activeAbilities = [];
    public IReadOnlyList<IAbility> Abilities => _abilities;

    private readonly TaskCompletionSource _initialized =
        new(TaskCreationOptions.RunContinuationsAsynchronously);

    public Task Initialization => _initialized.Task;


    public override void _Ready()
    {
        this.GetComponents(_abilities);

        if (!this.TryGetComposedRecursive(out IEntity? entity))
            return;
        
        _entity = entity;

        Enable();
        _initialized.TrySetResult();
    }

    public bool Select(int index)
    {
        if (!Enabled)
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
        if (!Enabled)
            return false;
            
        if (index >= _activeAbilities.Count)
            return false;

        if (!_activeAbilities[index].Cancel(_entity))
            return false;

        _activeAbilities.RemoveAt(index);
        return true;
    }

    protected override void EnableSpec()
    {
        foreach (IAbility ability in _abilities)
            ability.Enable();
    }

    protected override void DisableSpec()
    {
        foreach (IAbility ability in _abilities)
            ability.Disable();

        _activeAbilities.Clear();
    }
}