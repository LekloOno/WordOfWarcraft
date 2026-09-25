using System;
using Godot;
using WowGd.Src.Combat.Abilities.CoolDowns.EventData;
using WowGd.Src.Combat.Abilities.Data;
using WowGd.Src.Tools;

namespace WowGd.Src.Combat.Abilities.CoolDowns;

public class CoolDown(ICoolDownData data) : ICoolDown
{
    public ICoolDownData CoolDownData => data;
    
    public ulong Remaining => Completed() ? 0 : RemainingUnchecked;

    /// <summary>
    /// Only use in context where it is sure the current time does not overflow the deadline. 
    /// </summary>
    private ulong RemainingUnchecked => DeadLine - Time.GetTicksMsec();

    private ulong _requestedCd;
    private ulong _lastStart;

    private ulong DeadLine => _requestedCd + _lastStart;

    public event Action<CoolDownEventData>? CdStartedAt;
    public event Action<CoolDownModification>? CdReduced;
    public event Action<CoolDownModification>? CdEnlenghted;
    public event Action<CoolDownCompletion>? CdCompleted;

    private SceneTreeTimer? _completionTimer;

    public bool Completed() =>
        DeadLine <= Time.GetTicksMsec();

    public void CancelCd()
    {
        if (Completed())
            return;

        Complete(new CoolDownCompletion(true));
    }

    private void Complete() => Complete(new(false));
    private void Complete(CoolDownCompletion completion)
    {
        if (_completionTimer != null)
        {
            _completionTimer.Timeout -= Complete;
            _completionTimer.TimeLeft = 0f;
            _completionTimer = null;
        }

        _lastStart = 0;
        _requestedCd = 0;
        CdCompleted?.Invoke(completion);
    }

    public void EnlengthCd(ulong timeMs)
    {
        if (Completed())
            return;

        ulong prev = _requestedCd;
        _requestedCd += timeMs;
        CdEnlenghted?.Invoke(new(CoolDownData.Base, _requestedCd, prev, timeMs));
    }

    public void ReduceCd(ulong timeMs)
    {
        if (Completed())
            return;

        ulong prev = _requestedCd;
        bool completed = RemainingUnchecked <= timeMs;

        // The weird double completed branching is just to ensure CdReduced is casted before CdCompleted.
        // It feels more intuitive to have the sequence in that order.
        // The reduction is CAUSING the completion.
        
        if (completed)
            _requestedCd = 0;
        else
            _requestedCd -= timeMs;
        
        CdReduced?.Invoke(new(CoolDownData.Base, _requestedCd, prev, timeMs));
        if (completed)
            Complete(new CoolDownCompletion(timeMs - RemainingUnchecked));
    }

    public void StartCd() =>
        StartCdAt(CoolDownData.Base);

    public void StartCdAt(ulong timeMs)
    {
        if (timeMs == 0)
            return;

        _requestedCd    = timeMs;
        _lastStart      = Time.GetTicksMsec();

        CdStartedAt?.Invoke(new(CoolDownData.Base, _requestedCd));
        
        if (_completionTimer != null)
        {
            _completionTimer.Timeout -= Complete;
            _completionTimer.TimeLeft = 0f;
            _completionTimer = null;
        }

        if (!StaticTree.TryGetTree(out SceneTree? tree))
            return;

        _completionTimer = tree.CreateTimer(timeMs/1000f);
        _completionTimer.Timeout += Complete;
    }
}