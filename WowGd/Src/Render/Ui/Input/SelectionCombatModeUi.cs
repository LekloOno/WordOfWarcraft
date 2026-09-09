using System.Collections.Generic;
using Godot;
using WowGd.Src.Combat.Abilities;
using WowGd.Src.Combat.Modes;
using WowGd.Src.Input;
using WowGd.Src.Input.Hands;

namespace WowGd.Src.Render.Ui.Input;

[GlobalClass]
public partial class SelectionCombatModeUi : Control, IHandInputUi
{
    [Export] private SelectionCombatMode _selectionMode = null!;
    [Export] private SelectionCombatModeInput _selectionModeInput = null!;
    [Export] private Container _abilitiesContainer = null!;
    [Export] private PackedScene _cellUiTemplate = null!;

    private readonly List<AbilityCellUi> _abilityCellUis = [];


    public IHandsInputUiContext Context => throw new System.NotImplementedException();
    public HandsEnum        Hand    => HandsEnum.Second;
    public HandInputDocking Docking => HandInputDocking.Main;
    public IListenableHandInputMode InputMode => _selectionModeInput;
    public Control ControlNode => this;

    public void HideHand()
    {
        Hide();

    }

    public void ShowHand()
    {
        Show();
    }

    public void SetActive()
    {
        throw new System.NotImplementedException();
    }

    public void SetUnactive()
    {
        throw new System.NotImplementedException();
    }

    private void Sync()
    {
        foreach (Node node in _abilitiesContainer.GetChildren())
            node.QueueFree();

        for (int i = 0; i < _selectionMode.Abilities.Count; i++)
        {
            IAbility ability = _selectionMode.Abilities[i];
            if (ability is not IListenableAbility listenableAbility)
            {
                GD.PushWarning($"Ability {ability} in selection mode does not implement {nameof(IListenableAbility)}, which is required for {nameof(SelectionCombatModeUi)} to work properly.");
                continue;
            }

            var cell = _cellUiTemplate.Instantiate<AbilityCellUi>();
            cell.Ability = listenableAbility;
            cell.SetIndex(i);
            _abilityCellUis.Add(cell);
            AddChild(cell);
        }
    }
}
