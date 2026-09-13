using Godot;

namespace WowGd.Src.Render.Ui.Combat.Resources.Indicators.Standard;

[GlobalClass]
public partial class StdResIndicator2D : ResIndicator2D<StdResIndicatorColor>
{
    [Export] private StdIndicatorRelationColors _relationColors = null!;
    public override IResIndicatorRelationColors<StdResIndicatorColor> RelationColors => _relationColors;
}