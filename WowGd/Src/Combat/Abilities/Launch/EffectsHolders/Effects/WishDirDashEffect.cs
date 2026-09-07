using Godot;
using WowGd.Src.Combat.Abilities.Targeting.Payload;

namespace WowGd.Src.Combat.Abilities.Launch.EffectsHolders.Effects;

[GlobalClass]
public partial class WishDirDashEffect : Effect
{
    public override string Id => "wish_dir_effect_dash";
    [Export] private float _stength = 5f;

    public override bool Apply(TargetsPayload targetsPayload)
    {
        targetsPayload.Launcher.Body.ApplyImpulse(_stength * targetsPayload.Launcher.WishDir.WishDir());
        return true;
    }
}