using System.Collections.Generic;
using WowGd.Src.Render.Ui.Input.HandStack;

namespace WowGd.Src.Render.Ui.Input;

/// <summary>
/// The contexts uses multiple ui stacks.
/// 
/// There's two layer of separation -
/// First, active layers.
/// Some layers might be visible but unactive (which might come with special stylization).
/// 
/// Then, there's the docking layer = visibility layer.
/// Input UIs can be splitted in different docks. The element at the top of a given dock is always the visible one.
/// 
/// The stack themselves are of course divided in two hands.
/// 
/// To understand the difference - here is a little example.
/// Spell selection and targeting are both second handed input action.
/// 
/// However, spell selection is tied to the main dock, while targeting is a dock-free UI.
/// So, if selection is the top input UI, it will be visible, and fully active.
/// 
/// If we now push the targeting ui, it will not be pushed in the main dock, since it's dock-free.
/// Spell selection ui can remain visible, but it is set unactive, so we can for example grey it out, and disable the key-binds labels.
/// 
/// That's where active and visibility differs.
/// </summary>
public class HandsInputUiContext : IHandsInputUiContext
{
    public static readonly HandsInputUiContext Instance = new();

    private readonly ActiveHandStackUi _activeHandStack = new();
    private readonly Dictionary<HandInputDocking, DockHandStackUi> _dockHandStacks = new()
    {
        [HandInputDocking.Main] = new(),
        [HandInputDocking.Screen] = new(),
    };

    public bool TryPushUi(IHandInputUi handInputUi, HandInputDocking docking)
    {
        if (_dockHandStacks.TryGetValue(docking, out DockHandStackUi? stack))
            stack.PushUi(handInputUi);
        else if (docking != HandInputDocking.None)
            return false;

        _activeHandStack.PushUi(handInputUi);
        return true;
    }

    public bool TryRemoveUi(IHandInputUi handInputUi, HandInputDocking docking)
    {
        if (_dockHandStacks.TryGetValue(docking, out DockHandStackUi? stack))
            stack.RemoveUi(handInputUi);
        else if (docking != HandInputDocking.None)
            return false;

        _activeHandStack.RemoveUi(handInputUi);
        return true;
    }
}