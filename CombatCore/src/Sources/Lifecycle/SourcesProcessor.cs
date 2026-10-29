namespace CombatCore.Sources.Lifecycle;


/// <summary>
/// The only writer of the source tree. Single threaded, run-to-completion:
/// one command is fully applied (mutation, then notifications) before the next one starts.
/// </summary>
public sealed partial class SourcesProcessor(ISourceLifecycleSink? sink = null, Action<SourceCommand>? route = null)
{
    public Source Root { get; } = new(0, "System") { State = SourceState.Active };

    private readonly ISourceLifecycleSink _sink = sink ?? NullSink.Instance;

    /// <summary>
    /// null: standalone, the processor queues and drains its own commands.
    /// Not null: DRIVEN by an owner (the authority). A submitted command is not queued here, it is handed to the owner, which
    /// decides when and how it is applied (as a request of its own pipeline) and then calls <see cref="Apply"/>.
    /// Hooks and tickets keep working unchanged: whatever they submit reaches the owner.
    /// </summary>
    private readonly Action<SourceCommand>? _route = route;
    private readonly Queue<SourceCommand> _queue = new();
    private readonly List<Notification> _notes = [];
    private readonly List<Source> _scratch = [];
    private bool _draining;

    private sealed class NullSink : ISourceLifecycleSink
    {
        public static readonly NullSink Instance = new();
    }
}