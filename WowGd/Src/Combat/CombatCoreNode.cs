using System.Threading.Tasks;
using CombatCore;
using Godot;
using WowGd.Src.Entities;
using WowGd.Src.Tools;

namespace WowGd.Src.Combat;

public partial class CombatCoreNode : Node, IInitializable
{
    public static CombatCoreNode Instance { get; private set; } = null!;
    public static Authority Authority => Instance._authority;
    public Task Initialization => _initialized.Task;

    private readonly TaskCompletionSource _initialized =
        new(TaskCreationOptions.RunContinuationsAsynchronously);
    private readonly IGameClock _clock = new GameClockNode();
    private Authority _authority = null!;

    public override async void _Ready()
    {
        await EntitiesRegistryNode.Instance.Initialization;

        _authority = new(_clock, EntitiesRegistryNode.Registry);

        Instance = this;

        _initialized.TrySetResult();
    }
}