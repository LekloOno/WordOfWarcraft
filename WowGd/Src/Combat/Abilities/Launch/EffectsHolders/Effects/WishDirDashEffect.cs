using Godot;
using WowGd.Src.Combat.Abilities.Targeting.Payload;

namespace WowGd.Src.Combat.Abilities.Launch.EffectsHolders.Effects;

[GlobalClass]
public partial class WishDirDashEffect : Effect
{
    public override string Id => "wish_dir_effect_dash";
    [Export] public DashData DashData
    {
        get => _dashData;
        set
        {
            if (_dashData == value)
                return;

            _dashData = value;
            _dash = new(_dashData);
        }
    }
    
    private DashData _dashData = null!;

    private Dash _dash = null!;

    public WishDirDashEffect() { _dash = new(_dashData); }

    public override bool Apply(TargetsPayload targetsPayload) =>
        _dash.StartTo(targetsPayload.Launcher.WishDir.WishDir(), targetsPayload.Launcher.EntityMover);
}