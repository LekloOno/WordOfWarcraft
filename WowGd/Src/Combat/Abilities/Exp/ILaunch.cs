using WowGd.Src.Tools;

namespace WowGd.Src.Combat.Abilities.Exp;

public interface ILaunch : IDisablable
{
    void Start(CastPayload payload);
    void Stop();
}