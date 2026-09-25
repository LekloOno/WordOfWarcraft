using System.Collections.Generic;
using Godot;
using WowGd.Src.Combat.Abilities;
using WowGd.Src.Combat.Modes;
using WowGd.Src.Input;
using WowGd.Src.Input.Hands;
using WowGd.Src.Render.WorldRenderer.Entities;

namespace WowGd.Src.Render.Ui.Input.Impls;

[GlobalClass]
public partial class SelectionCombatModeUi : Node, IHandInputUi
{
    private SelectionCombatMode SelectionMode => _selectionModeInput.Mode;

    private SelectionCombatModeInput _selectionModeInput = null!;
    [Export] public SelectionCombatModeInput SelectionModeInput
    {
        get => _selectionModeInput;
        set
        {
            if (value == _selectionModeInput)
                return;

            if (_selectionModeInput != null)
                this.UnbindToContext();

            _selectionModeInput = value;
            if (_selectionModeInput != null)
                this.BindToContext();
        }
    }

    [Export] private Container _abilitiesContainer = null!;
    [Export] private PackedScene _cellUiTemplate = null!;

    private readonly List<AbilityCellUi> _abilityCellUis = [];


    public IHandsInputUiContext Context => HandsInputUiContext.Instance;
    public HandInputDocking Docking => HandInputDocking.Main;
    public IListenableHandInputMode InputMode => _selectionModeInput;
    public Control ControlNode => _abilitiesContainer;

    public override async void _Ready()
    {
        HideHand();
        SetUnactive();
        PlayerInputUiSyncer.Register(this);

        await SelectionMode.Initialization;
        Sync();
    }

    public void HideHand()
    {
        _abilitiesContainer.Hide();
    }

    public void ShowHand()
    {
        _abilitiesContainer.Show();
    }

    public void SetActive()
    {
        foreach (AbilityCellUi cell in _abilityCellUis)
            cell.SetActive();
    }

    public void SetUnactive()
    {
        foreach (AbilityCellUi cell in _abilityCellUis)
            cell.SetUnactive();
    }

    private void Sync()
    {
        _abilityCellUis.Clear();
        
        int idx = -1;
        foreach (Node node in _abilitiesContainer.GetChildren())
        {
            if (node is not AbilityCellUi cell)
                continue;

            idx ++;

            if (idx >= SelectionMode.Abilities.Count)
                return;

            if (SelectionMode.Abilities[idx] is not IAbility listenable)
            {
                GD.PushWarning($"Ability {SelectionMode.Abilities[idx]} in selection mode does not implement {nameof(IAbility)}, which is required for {nameof(SelectionCombatModeUi)} to work properly.");
                continue;
            }

            cell.SetIndex(idx);
            cell.Ability = listenable;
            _abilityCellUis.Add(cell);
        }
    }
}
