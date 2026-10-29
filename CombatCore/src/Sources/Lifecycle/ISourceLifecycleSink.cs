namespace CombatCore.Sources.Lifecycle;


/// <summary>Observer of lifecycle facts. This is what will be bridged to authority facts.</summary>
public interface ISourceLifecycleSink
{
    void OnInserted(Source parent, Source child) { }
    void OnTerminationStarted(Source node) { }
    void OnTerminated(Source node, bool killed) { }
    void OnAborted(Source node) { }
    void OnRejected(SourceCommandKind kind, Source node, SourceRejectReason reason) { }
}