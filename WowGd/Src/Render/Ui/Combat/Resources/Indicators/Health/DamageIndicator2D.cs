using Godot;

namespace WowGd.Src.Render.Ui.Combat.Resources.Indicators.Health;

[GlobalClass]
public partial class DamageIndicator2D : ResIndicator2D<DamageIndicatorColor>, IDamageIndicator
{
    [Export] private DamageIndicatorRelationColors _relationColors = null!;
    public override IResIndicatorRelationColors<DamageIndicatorColor> RelationColors => _relationColors;

    [Export] private float _deadScale       = 1f;
    [Export] private float _resurrectScale  = 1f;

    public void OnDied()
    {
        SetText("Dead");
        Modulate = _activeColors.DeadColor;
        StartAnimation(_deadScale);
    }
    
    public void OnResurrected(int hp)
    {
        SetText("Resurrected");
        Modulate = _activeColors.ResColor;
        StartAnimation(_resurrectScale);
    }
}