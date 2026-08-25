using Godot;
using WowGd.Src.Dactylo.Abilities.Effects;
using WowGd.Src.Dactylo.Worders;
using WowGd.Src.Entities;
using WowGd.Src.Tools;

namespace WowGd.Src.Dactylo.Abilities;

[GlobalClass]
public partial class WordBasedAbility : Node, IWorderStreamHandler
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

	public override void _Ready()
	{
		if (!this.TryGetComponent(out _worder!))
			return;

		if (!this.TryGetComposedRecursive(out _entity!))
			return;

		this.Bind(_worder);
		_worder.TryGenerate(20, _wordMinSize, _wordMaxSize);
		_worder.Next();
	}

	public void OnCompleted(string written, string target, int correct)
	{
		float accuracy   = (float) correct/target.Length;
		float sizeFactor = (float) target.Length / _wordBaseSize;

		_effect.Apply(_entity, new(Target, _targetTeamMask), accuracy * sizeFactor);
	}

	public void OnWordStarted(string word, int remainingWords)
	{
		if (remainingWords == 0)
			_worder.TryGenerate(20, _wordMinSize, _wordMaxSize);
	}
}
