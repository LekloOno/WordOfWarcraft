using CombatCore;
using Godot;

namespace WowGd.Src.Combat;

public partial class GameClockNode : IGameClock
{
    public ulong Tick => Engine.GetPhysicsFrames();
}