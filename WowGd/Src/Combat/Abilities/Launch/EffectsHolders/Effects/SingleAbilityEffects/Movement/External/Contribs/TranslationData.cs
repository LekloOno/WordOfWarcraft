using Godot;
using WowGd.Src.Physics.Movement;

namespace WowGd.Src.Combat.Abilities.Launch.EffectsHolders.Effects.SingleAbilityEffects.Movement.External.Contribs;

[GlobalClass]
public partial class TranslationData : Resource, IMoveContribData
{
    [Export(PropertyHint.Range, "0,15,0.1,or_greater,exp")]
    private float _distance = 2f;
    [Export(PropertyHint.Range, "0,2,0.1,or_greater,exp")]
    private float _duration = 0.2f;
    [Export] private TranslationWeighting _weighting =
        TranslationWeighting.Distance | TranslationWeighting.Duration;

    public bool Start(IEntityMover mover, Vector2 direction, float weight, bool strict = false)
    {
        float distance = _distance;
        if (_weighting.HasFlag(TranslationWeighting.Distance))
            distance *= weight;

        float duration = _duration;
        if (_weighting.HasFlag(TranslationWeighting.Duration))
            duration *= weight;

        return new TranslationContrib(mover, direction, distance, duration, strict).Start();
    }
}