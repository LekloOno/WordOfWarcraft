using WowGd.Src.Combat.Abilities.Actuation;

namespace WowGd.Src.Combat.Abilities.Launch;

public interface ILaunch
{
    void Launch(ActuatePayload payload);
}