using Godot;

namespace WowGd.Src.Render.Ui.Combat.Resources.Indicators.Health;

[GlobalClass]
public partial class DamageIndicatorRelationColors : Resource, IResIndicatorRelationColors<DamageIndicatorColor>
{
    [Export] public DamageIndicatorColor Self     { get; private set; } = null!;
    [Export] public DamageIndicatorColor Ally     { get; private set; } = null!;
    [Export] public DamageIndicatorColor Enemy    { get; private set; } = null!;
}