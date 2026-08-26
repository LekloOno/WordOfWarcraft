using Godot;
using WowGd.Src.Combat.Health;

namespace WowGd.Src.Render.Ui.Combat.Health;

public interface IDamageIndicator : IEntityHealthHandler
{
    void SetWorldPosition(Vector3 worldPosition);
}