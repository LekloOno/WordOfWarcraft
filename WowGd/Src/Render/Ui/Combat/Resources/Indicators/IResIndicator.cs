using Godot;
using WowGd.Src.Combat.Abilities.Targeting.Payload;
using WowGd.Src.Combat.Resources;

namespace WowGd.Src.Render.Ui.Combat.Resources.Indicators;

public interface IResIndicator<T> : IStandardResourceHandler where T : IResIndicatorColor
{
    void SetWorldPosition(Vector3 worldPosition);
    void SetClientRelation(TargetRelation relation);
    IResIndicatorRelationColors<T> RelationColors { get; }
    void SetText(string text);
}