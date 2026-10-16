using System.Threading.Tasks;
using Godot;
using WowGd.Src.Tools;

namespace WowGd.Src.Entities;

public partial class EntitiesRegistryNode : Node, IInitializable
{
    private readonly EntitiesRegistry _registry = new();
    private readonly TaskCompletionSource _initialized =
        new(TaskCreationOptions.RunContinuationsAsynchronously);
    
    public static EntitiesRegistryNode Instance { get; private set; } = null!;
    public Task Initialization => _initialized.Task;
    public static EntitiesRegistry Registry => Instance._registry;


    public override void _EnterTree()
    {
        Instance = this;
        _initialized.TrySetResult();
    }
}