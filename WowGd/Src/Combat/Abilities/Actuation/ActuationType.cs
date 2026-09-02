namespace WowGd.Src.Combat.Abilities.Actuation;

// Having display bind data logic in a domain that should be pure logic
// might smell like code smell.
// It is a bit tbh, but it's a decent compromise for now,
// especially since it's not purely visual, such aspects have gameplay impact.
// The otherway around would be to share the IActuator itself or its id,
// and have a registry resolve the proper display methods from type/id.

public enum ActuationType
{
    /// <summary>
    /// A cast to feed and release
    /// </summary>
    Cast,
    /// <summary>
    /// Simple word(s) correspondance
    /// </summary>
    Word,
    /// <summary>
    /// Multiple successive words
    /// </summary>
    Fury,
    /// <summary>
    /// Continuous
    /// </summary>
    Beam,
}