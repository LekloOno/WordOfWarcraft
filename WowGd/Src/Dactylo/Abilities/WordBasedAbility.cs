using System.Collections.Generic;
using Godot;
using WowGd.Src.Combat.Abilities.Effects;
using WowGd.Src.Combat.Abilities.Targets.Payload;
using WowGd.Src.Dactylo.Worders;
using WowGd.Src.Entities;
using WowGd.Src.Tools;

namespace WowGd.Src.Dactylo.Abilities;

[GlobalClass]
public partial class WordBasedAbility : Node, IWorderStreamHandler, IDisablable
{
	[Export] private AbilityEffect _effect = null!;
	[Export(PropertyHint.Layers2DRender)]
	private uint _targetTeamMask;

	[Export] private int _wordMinSize = 1;
	[Export] private int _wordMaxSize = 10;
	[Export] private int _wordBaseSize = 5;

	private IWorder _worder = null!;
	private IEntity _entity = null!;
	public Vector2 Target = Vector2.Zero;

	public bool Enabled => _enabled;
	private bool _enabled = false;

	public override void _Ready()
	{
		if (!this.TryGetComponent(out _worder!))
			return;

		if (!this.TryGetComposedRecursive(out _entity!))
			return;

		this.Bind(_worder);
		_worder.Next();
	}

	public void OnCompleted(string written, string targetWord, int correct)
	{
		float accuracy   = (float) correct/targetWord.Length;
		float sizeFactor = (float) targetWord.Length / _wordBaseSize;

		// TEMPORARY
		// Targeting logic should later be externalized
		ICollection<IEntity> entities = _entity.GetTargetsInRange(5f, excludeSelf: false);

		HashSet<Target> targets = [];
		foreach (IEntity entity in entities)
		{
			Target target;

			if (entity == _entity)
				target = new(entity, TargetRelation.Self, false);
			else if (entity.TeamMask == _entity.TeamMask)
				target = new(entity, TargetRelation.Ally, false);
			else
				target = new(entity, TargetRelation.Enemy, false);
			
			targets.Add(target);
		}

		TargetsPayload payload = new(targets, _entity);
		_effect.Apply(payload, accuracy * sizeFactor);
	}

	public void OnWordStarted(string word, int remainingWords) {}

	public bool Enable() =>
		DisableExt.IndempEnable(ref _enabled, () => _worder.Enable());
	public bool Disable() =>
		DisableExt.IndempDisable(ref _enabled, () => _worder.Disable());
}
