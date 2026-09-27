using System;

namespace WowGd.Src.Combat.Abilities.Launch.EffectsHolders.Gatherers;

[Flags]
public enum RadiusGathererSettings
{
    WeightOnDistance    = 1 << 0,
    IncludeDirect       = 1 << 1, 
    SelfObstruction     = 1 << 2,   // Not implemented yet
    AlliesObstruction   = 1 << 3,   // Not implemented yet
    EnemiesObstruction  = 1 << 4,   // Not implemented yet
    WallsObstruction    = 1 << 5,   // Not implemented yet
}