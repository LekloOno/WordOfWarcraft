using WowGd.Src.Combat.Abilities.Actuation;

namespace WowGd.Src.Combat.Abilities.Data;

public interface ILoopRule
{
    string Id { get; }
    bool Check(ActuatePayload payload);
}