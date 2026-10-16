namespace CombatCore;

public readonly record struct StatId(uint Value);

/// <summary>Flat is scaled by the weight; Mult's delta (Mult - 1) is scaled by the weight, so weight 0 = no effect.</summary>
public readonly record struct StatContribution(StatId Stat, int Flat = 0, float Mult = 1f);
