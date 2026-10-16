using CombatCore;
using Godot;

namespace WowGd.Src.Combat;

public partial class GameClockNode : IGameClock
{
    public long Tick => (long) Engine.GetPhysicsFrames();
}