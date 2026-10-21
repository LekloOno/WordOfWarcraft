namespace CombatCore.Status;

/// <summary>
/// The statuses of one entity. A single list is the source of truth: typed access is just "find the instance of this
/// definition and look at its Data", so there is no second structure to keep in sync.
/// </summary>
internal sealed class StatusRegistry
{
    private readonly List<StatusInstance> _instances = [];

    internal IReadOnlyList<StatusInstance> Instances => _instances;

    internal void Register(StatusInstance instance) => _instances.Add(instance);

    internal bool Remove(StatusInstance instance) => _instances.Remove(instance);
}
