using Godot;
using Godot.Collections;
using WowGd.Src.Combat.Abilities.Exp.Actuation;
using WowGd.Src.Combat.Abilities.Exp.CasterRules;
using WowGd.Src.Combat.Abilities.Exp.Data;
using WowGd.Src.Combat.Abilities.Exp.Launch.EffectsHolders;

namespace WowGd.Src.Combat.Abilities.Exp.Launch;

public partial class Launch : Node, ILaunch
{
    // We could get a reference to the data, to make rules editable.
    // But since effect holders depend on logic structure, and are thus not editable yet,
    // I think it's clearer if nothing is editable.

    // We'll make rules editable when logic structure will be too.

    // The data contained inside the rules themselves should remain editable though !
    // Just not the list itself.
    private readonly CasterRule[]       _casterRules    = [];
    private readonly TargetRule[]       _targetRules    = [];
    private readonly IEffectsHolder[]   _effectHolders  = [];
    
    public Launch() {}
    public Launch(Array<CasterRule> casterRules, Array<TargetRule> targetRules, Array<EffectsHolderData> effectsHolders)
    {
        _casterRules = [.. casterRules];
        _targetRules = [.. targetRules];
        
        int count = effectsHolders.Count;

        _effectHolders = new IEffectsHolder[count];
        for (int i = 0; i < count; i++)
            _effectHolders[i] = effectsHolders[i].Build();
    }

    void ILaunch.Launch(ActuatePayload payload)
    {
        if (!_casterRules.CheckAll(payload.Caster))
            return;

        if (!_targetRules.CheckAll(payload.Caster, payload.Intent))
            return;

        foreach (IEffectsHolder holder in _effectHolders)
            holder.Launch(payload);
    }
}