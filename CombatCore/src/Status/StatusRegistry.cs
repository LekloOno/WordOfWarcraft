using System.Diagnostics.CodeAnalysis;

namespace CombatCore.Status;

internal sealed class StatusRegistry
{
    private readonly List<StatusInstance> _generalStatuses = [];
    private readonly Dictionary<Type, object> _typedStatuses = [];
    private readonly Dictionary<StatusInstance, Type> _statusTypes =
        new(ReferenceEqualityComparer.Instance);

    internal IReadOnlyList<StatusInstance> GeneralStatuses => _generalStatuses;

    internal bool TryGet<T>([NotNullWhen(true)] out T? status)
    {
        if (_typedStatuses.TryGetValue(typeof(T), out object? s))
        {
            status = (T)s;
            return true;
        }

        status = default;
        return false;
    }

    internal void Register(StatusInstance instance) =>
        _generalStatuses.Add(instance);

    internal bool Register<T>(T instance, StatusInstance? baseInstance) where T : notnull
    {
        if (!_typedStatuses.TryAdd(typeof(T), instance))
            return false;

        if (baseInstance is not null)
        {
            _generalStatuses.Add(baseInstance);
            _statusTypes.Add(baseInstance, typeof(T));
        }

        return true;
    }

    internal bool Remove<T>() where T : notnull
    {
        bool typed = _typedStatuses.Remove(typeof(T), out object? status);

        bool general;

        if (status is StatusInstance instance)
        {
            _generalStatuses.Remove(instance);
            _statusTypes.Remove(instance);
            general = true;
        }
        else
            general = false;

        return typed || general;
    }

    internal bool Remove(StatusInstance instance)
    {
        bool general = _generalStatuses.Remove(instance);

        bool typed = _statusTypes.Remove(instance, out Type? type);

        if (typed)
            _typedStatuses.Remove(type!);

        return typed || general;
    }
}