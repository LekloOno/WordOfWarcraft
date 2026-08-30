using WowGd.Src.Combat.Abilities.Exp.Actuation;
using WowGd.Src.Tools;

namespace WowGd.Src.Combat.Abilities.Exp;

public interface ILaunch : IDisablable
{
    void Start(ActuatePayload payload);
    void Stop();
}