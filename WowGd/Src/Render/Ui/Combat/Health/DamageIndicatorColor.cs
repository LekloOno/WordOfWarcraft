using Godot;

[GlobalClass]
public partial class DamageIndicatorColor : Resource
{
    [Export] public Color DamageColor   { get; private set; }
    [Export] public Color HealColor     { get; private set; }
    [Export] public Color ResColor      { get; private set; }
    [Export] public Color DeadColor     { get; private set; }
}