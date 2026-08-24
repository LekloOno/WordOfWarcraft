using System;
using Godot;
using WowGd.Src.Input;
using WowGd.Src.Tools;

namespace WowGd.Src.Debug;

[GlobalClass]
public partial class RandomMode : Node, IMode
{
    [Export] private TextEdit _textEdit = null!;

    public bool Enabled => _enabled;
    private bool _enabled = true;
    private bool _active = false;

    public override void _Ready()
    {
        ModeRegistry.Register(Key.K, this);
    }

    public bool Activate() =>
        DisableExt.IndempActivate(ref _active, _enabled, _textEdit.GrabFocus);

    public bool Deactivate() =>
        DisableExt.IndempDeactivate(ref _active, _enabled, _textEdit.ReleaseFocus);

    public bool Enable() =>
        DisableExt.IndempEnableActivable(ref _enabled, _active, _textEdit.GrabFocus);

    public bool Disable() =>
        DisableExt.IndempDisable(ref _enabled, _textEdit.ReleaseFocus);
}