using WowGd.Src.Combat.Abilities.Exp.Actuation;

namespace WowGd.Src.Combat.Abilities.Exp.Launch;

public interface ILaunch
{
    void Launch(ActuatePayload payload);
}