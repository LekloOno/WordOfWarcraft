using System.Collections.Generic;
using Godot;
using WowGd.Src.Dactylo.Abilities.Effects;
using WowGd.Src.Dactylo.Generators;
using WowGd.Src.Dactylo.Worders;
using WowGd.Src.Entities;
using WowGd.Src.Tools;

namespace WowGd.Src.Dactylo.Abilities;

[GlobalClass]
public partial class WordBasedAbility : Node, IWorderStreamHandler, IAbility
{
	[Export] private FreeTargetAbilityEffect _effect = null!;
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

	public void OnCompleted(string written, string target, int correct)
	{
		float accuracy   = (float) correct/target.Length;
		float sizeFactor = (float) target.Length / _wordBaseSize;

		_effect.Apply(_entity, new(Target, _targetTeamMask), accuracy * sizeFactor);
	}

	public void OnWordStarted(string word, int remainingWords) {}

	public bool Enable() =>
		DisableExt.IndempEnable(ref _enabled, () => _worder.Enable());
	public bool Disable() =>
		DisableExt.IndempDisable(ref _enabled, () => _worder.Disable());
}
