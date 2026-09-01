using WowGd.Src.Combat.Abilities.Exp.Actuation;
using WowGd.Src.Combat.Abilities.Exp.Data;
using WowGd.Src.Combat.Abilities.Exp.Launch.EffectsHolders;

namespace WowGd.Src.Combat.Abilities.Exp.Launch;

public class Launch : ILaunch
{
    // We could get a reference to the data, to make rules editable.
    // But since effect holders depend on logic structure, and are thus not editable yet,
    // I think it's clearer if nothing is editable.

    // We'll make rules editable when logic structure will be too.

    // The data contained inside the rules themselves should remain editable though !
    // Just not the list itself.
    private readonly LaunchData         _data;
    private readonly IEffectsHolder[]   _effectHolders  = [];
    
    public Launch(LaunchData data)
    {
        _data = data;
        
        int count = data.EffectsHoldersDt.Count;

        _effectHolders = new IEffectsHolder[count];
        for (int i = 0; i < count; i++)
            _effectHolders[i] = data.EffectsHoldersDt[i].Build();
    }

    void ILaunch.Launch(ActuatePayload payload)
    {
        if (!_data.CasterRulesDt.CheckAll(payload.Caster))
            return;

        if (!_data.TargetRulesDt.CheckAll(payload.Caster, payload.Intent))
            return;

        foreach (IEffectsHolder holder in _effectHolders)
            holder.Launch(payload);
    }
}