namespace CombatCore.Sources.Lifecycle;

public sealed partial class SourcesProcessor
{
    private void Submit(in SourceCommand command)
    {
        if (_route is not null) { _route(command); return; }   // driven: the owner decides when
        _queue.Enqueue(command);
        if (_draining) return; // called from a hook: it will run after the current command
        Drain();
    }

    /// <summary>
    /// Driven mode: applies exactly this command now (mutation, then its notifications), run to completion.
    /// Whatever the hooks submit meanwhile goes through the route, so commands never nest.
    /// </summary>
    public void Apply(in SourceCommand command)
    {
        _draining = true;
        try
        {
            Execute(command);
            Flush(command.Ctx);
        }
        finally
        {
            _draining = false;
            _notes.Clear();
        }
    }

    private void Drain()
    {
        _draining = true;
        try
        {
            while (_queue.TryDequeue(out var command))
            {
                Execute(command);
                Flush(command.Ctx);
            }
        }
        finally
        {
            _draining = false;
            _notes.Clear();
        }
    }

    private void Execute(in SourceCommand c)
    {
        switch (c.Kind)
        {
            case SourceCommandKind.Insert: ExecuteInsert(c.Node, c.Other!); break;
            case SourceCommandKind.Terminate: ExecuteTerminate(c.Node); break;
            case SourceCommandKind.Kill: ExecuteKill(c.Node); break;
            case SourceCommandKind.Abort: ExecuteAbort(c.Node); break;
            case SourceCommandKind.CompleteTermination: ExecuteComplete(c.Node, c.Generation); break;
        }
    }
}