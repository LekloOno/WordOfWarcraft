using Godot;
using WowGd.Src.Combat.Health;
using WowGd.Src.Render.Animation.Shakes;
using WowGd.Src.Tools;
using WowGd.Src.Tools.Curve.Float;

namespace WowGd.Src.Render.Camera;

[GlobalClass]
public partial class DamageShakesNode : Node
{
    [Export] public FloatCurveSampler _curveSampler = null!;

    private ITraumaChannel? _traumaChannel;
    private DamageShakes? _damageShakes;

    public void Init(IEntityHealth health, ITraumaChannel? traumaChannel = null)
    {
        traumaChannel ??= _traumaChannel;
        if (traumaChannel == null &&
            !this.TryGetComposed(out traumaChannel))
            return;

        _traumaChannel = traumaChannel;
        
        _damageShakes?.Dispose();
        _damageShakes = new(_curveSampler, health, traumaChannel);
    }
}