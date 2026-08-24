
using Godot;
using WowGd.Src.Tools;

namespace WowGd.Src.Input;

[GlobalClass]
public partial class ModeInitializer : Node
{
    [Export] private ModePicker _picker = null!;
    public override void _Ready()
    {
        if (this.TryGetComposed(out IMode? mode))
            _picker.Preselect(mode);
    }
}