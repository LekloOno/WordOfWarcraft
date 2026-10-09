using System;
using Godot;
using WowGd.Src.Entities;
using WowGd.Src.Entities.Stats.Tackle;
using WowGd.Src.Tools;

namespace WowGd.Src.Physics.Movement.Channels.Internal.Tackle;

public partial class DynamicTackleNode : Node
{
    public const float TackleGrace = 2f;

    private double _acc         = 0;
    private double _stamina     = 0;
    private ulong _lastDodge    = 0;

    public event Action<StaminaChange>? StaminaChanged;
    /// <summary>
    /// Occurs when the ongoing tackling interraction this node is being bound to changes.
    /// </param>
    public event Action<TackleInteraction>? TackleChanged;

    public override void _PhysicsProcess(double delta)
    {
        _acc += delta;

        if (!PhysicsExt.IsProcessTick(0xf, 1))
            return;

        var tacklers = _tackleNode.Tacklers;

        for (int i = tacklers.Count - 1; i >= 0; i--)
        {
            if (i >= tacklers.Count) continue;

            var tackler = tacklers[i];
            if (tackler.Entity.DistanceSquaredTo(Entity) > tackler.Entity.EntityMover.DynamicTackleNode._maxRangeSquared)
                tackler.Release();
        }

        if (!IsTackled) return;

        double tackleWeight = TackleStatisticsExt.GetTackleWeight(TotalEnduredTackle(), Entity.Dodge());
        double decay = _acc / tackleWeight;

        _acc = 0f;

        ApplyStamina(StaminaChangeKind.Tick, -decay);
    }

    private int TotalEnduredTackle()
    {
        int total = 0;
        foreach (TackleNode tackler in _tackleNode.Tacklers)
            total += tackler.Entity.Tackle();

        return total;
    }

    private bool CanGetTackled() => (Engine.GetPhysicsFrames() - _lastDodge) / (float) Engine.PhysicsTicksPerSecond > TackleGrace;
    public int DodgeTacklers() => _tackleNode.DodgeTacklers();

    public void DrainStaminaWeighted(double weight)
    {
        double amount = TackleStatisticsExt.GetTackleDrain(TotalEnduredTackle(), Entity.Dodge(), weight);
        DrainStamina(amount);
    }

    public void DrainStamina(double amount)
    {
        if (IsTackled)
            ApplyStamina(StaminaChangeKind.Drain, -Math.Max(amount, 0));
    }

    public void FeedStamina(double amount)
    {
        if (IsTackled)
            ApplyStamina(StaminaChangeKind.Feed, Math.Max(amount, 0));
    }

    private void ApplyStamina(StaminaChangeKind kind, double requested)
    {
        double prev = _stamina;
        _stamina = Math.Clamp(prev + requested, 0.0, 1.0);
        double delta = _stamina - prev;

        if (delta == 0 && kind != StaminaChangeKind.Tick)
            return;

        StaminaChanged?.Invoke(new(kind, delta, _stamina));

        if (_stamina > 0)
            return;

        DodgeTacklers();
        return;
    }

    private void OnGotTackled(TackledEventArgs args)
    {
        TackleInteractionType type = args.TacklerCount > 1
            ? TackleInteractionType.Tackled
            : TackleInteractionType.Initialized;

        if (type == TackleInteractionType.Initialized)
        {
            _stamina = 1f;
            _acc = 0f;

            SetPhysicsProcess(true);
        }

        TackleChanged?.Invoke(new(args, type));
    }

    private void OnGotReleased(TackledEventArgs args)
    {
        TackleInteractionType type = args.TacklerCount > 0
            ? TackleInteractionType.Released
            : TackleInteractionType.Freed;

        if (type == TackleInteractionType.Freed)
        {
            _lastDodge = Engine.GetPhysicsFrames();
            SetPhysicsProcess(false);
        }

        TackleChanged?.Invoke(new(args, type));
    }
}