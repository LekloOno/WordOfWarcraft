namespace WowGd.Src.Combat.Abilities.Actuation;

public readonly struct WordRequest(DeliveryMode mode, ActuationType display, int min, int max, int lookahead = 10, string flavorKey = "")
{
    /// <summary>
    /// Presentation information only.
    /// Minimum length of selected words.
    /// </summary>
    public readonly int     MinLength       = min;
    /// <summary>
    /// Presentation information only.
    /// Maximum length of selected words.
    /// </summary>
    public readonly int     MaxLength       = max;
    public readonly int     LookaheadCount  = lookahead;
    /// <summary>
    /// Presentation information only.
    /// Can be used by the client to pick up words in a specific bank.
    /// For AI, it might modulate their efficiency.
    /// </summary>
    public readonly string  FlavorKey       = flavorKey;
    public readonly DeliveryMode Mode       = mode;
    public readonly ActuationType Display   = display;
}