using Godot;
using WowGd.Src.Combat.Health;
using WowGd.Src.Combat.Resources;

namespace WowGd.Src.Render.Ui.Combat.Resources.Bars;

[GlobalClass]
public partial class HealthBar : StandardResourceBar, IEntityHealthHandler
{
	public void SetHealth(IEntityHealth health) => SetResource(health);

	protected override void BindResource(IStandardResource resource)
	{
		if (resource is IEntityHealth health)
			this.Bind(health);
		else
			base.BindResource(resource);
	}

	protected override void UnbindResource(IStandardResource resource)
	{
		if (resource is IEntityHealth health)
			this.Unbind(health);
		else
			base.UnbindResource(resource);
	}

	public void OnDied() {}
	public void OnResurrected(int hp) => OnGenerated(hp);
}
