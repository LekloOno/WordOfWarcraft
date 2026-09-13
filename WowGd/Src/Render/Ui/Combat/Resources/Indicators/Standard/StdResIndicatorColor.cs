using Godot;

namespace WowGd.Src.Render.Ui.Combat.Resources.Indicators.Standard;

[GlobalClass]
public partial class StdResIndicatorColor : Resource, IResIndicatorColor
{
    [Export] public Color ConsumedColor     { get; private set; }
    [Export] public Color GeneratedColor    { get; private set; }
}