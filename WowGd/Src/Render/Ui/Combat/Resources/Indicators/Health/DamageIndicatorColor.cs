using Godot;
using WowGd.Src.Render.Ui.Combat.Resources.Indicators.Standard;

namespace WowGd.Src.Render.Ui.Combat.Resources.Indicators.Health;

[GlobalClass]
public partial class DamageIndicatorColor : StdResIndicatorColor
{
    [Export] public Color ResColor      { get; private set; }
    [Export] public Color DeadColor     { get; private set; }
}