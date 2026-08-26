using System;
using System.Threading.Tasks;
using Godot;
using WowGd.Src.Combat.Abilities.Effects;
using WowGd.Src.Combat.Abilities.Targets.Payload;
using WowGd.Src.Combat.Abilities.Targets.Queriers;
using WowGd.Src.Dactylo.Worders;
using WowGd.Src.Entities;
using WowGd.Src.Tools;

namespace WowGd.Src.Dactylo.Abilities;

[GlobalClass]
public partial class WordBasedAbility : Node, IWorderStreamHandler, IDisablable
{
	[Export] private AbilityEffect _effect = null!;

	[Export] private int _wordMinSize  = 1;
	[Export] private int _wordMaxSize  = 10;
	[Export] private int _wordBaseSize = 5;

	private IWorder 		_worder 		= null!;
	private IEntity 		_entity 		= null!;
	private ITargetQuerier 	_targetQuerier 	= null!;
	public Vector2 Target = Vector2.Zero;

	public bool Enabled => _enabled;
	private bool _enabled = false;

	public override void _Ready()
	{
		if (!this.TryGetComponent(out _worder!))
			return;

		if (!this.TryGetComposedRecursive(out _entity!))
			return;

		if (!this.TryGetComponent(out _targetQuerier!))
			return;

		this.Bind(_worder);
		_worder.Next();
	}

	public void OnCompleted(string written, string targetWord, int correct) =>
		_ = TrySend(written, targetWord, correct);

	private async Task TrySend(string written, string targetWord, int correct)
	{
		try
		{
			TargetsPayload payload = await _targetQuerier.Query(_entity);
			
			int compSize	 = Math.Max(targetWord.Length, written.Length);
			float accuracy   = (float) correct / compSize;
			float sizeFactor = (float) targetWord.Length / _wordBaseSize;

			_effect.Apply(payload, accuracy * sizeFactor);
		}
		catch (Exception) {}
	}

	public void OnWordStarted(string word, int remainingWords) {}

	public bool Enable() =>
		DisableExt.IndempEnable(ref _enabled, () => _worder.Enable());
	public bool Disable() =>
		DisableExt.IndempDisable(ref _enabled, () => _worder.Disable());
}
