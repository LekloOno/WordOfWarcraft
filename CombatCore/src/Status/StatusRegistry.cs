namespace CombatCore.Status;

/// <summary>
/// The statuses of one entity: one dictionary by definition id, so lookups are O(1) and there is still a single structure
/// (nothing to keep in sync). The merge path guarantees at most one instance per definition per entity, and Register
/// fails fast if that invariant is ever broken. Enumeration order is not guaranteed: sort where order matters.
/// </summary>
internal sealed class StatusRegistry
{
    private readonly Dictionary<uint, StatusInstance> _byDefinition = [];

    internal IReadOnlyCollection<StatusInstance> Instances => _byDefinition.Values;

    internal StatusInstance? Find(uint definitionId) => _byDefinition.GetValueOrDefault(definitionId);

    internal void Register(StatusInstance instance) => _byDefinition.Add(instance.Def.Id, instance);

    internal bool Remove(StatusInstance instance) => _byDefinition.Remove(instance.Def.Id);
}
