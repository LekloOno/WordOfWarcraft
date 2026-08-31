using WowGd.Src.Combat.Abilities.Exp.Actuation;

namespace WowGd.Src.Combat.Abilities.Exp.Data;

public interface ILoopRule
{
    string Id { get; }
    bool Check(ActuatePayload payload);
}