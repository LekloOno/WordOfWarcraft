using System.Collections.Generic;
using Godot;
using WowGd.Src.Input;
using WowGd.Src.Input.Hands;

namespace WowGd.Src.Render.Ui.Input;

[GlobalClass]
public partial class SelectionCombatModeUi : Control, IHandInputUi
{
    [Export] private SelectionCombatModeInput _selectionMode = null!;
    [Export] private Container _abilitiesContainer = null!;
    // [Export] private WhateverHoldsAbilities;
    
    private List<AbilityCellUi> _abilityCellUis = [];


    public IHandsInputUiContext Context => throw new System.NotImplementedException();
    public HandsEnum        Hand    => HandsEnum.Second;
    public HandInputDocking Docking => HandInputDocking.Main;
    public IListenableHandInputMode InputMode => _selectionMode;
    public Control ControlNode => this;

    public void HideHand()
    {
        Hide();

    }

    public void ShowHand()
    {
        Show();
    }

    private void Sync()
    {
        // Sync abilities with sources..
    }
}
