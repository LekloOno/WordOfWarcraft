using Godot;

namespace WowGd.Src.Render.Ui.Combat.Resources.Indicators.Standard;

[GlobalClass]
public partial class StdIndicatorRelationColors : Resource, IResIndicatorRelationColors<StdResIndicatorColor>
{
    [Export] public StdResIndicatorColor Self     { get; private set; } = null!;
    [Export] public StdResIndicatorColor Ally     { get; private set; } = null!;
    [Export] public StdResIndicatorColor Enemy    { get; private set; } = null!;
}