using Godot;
using WowGd.Src.Combat.Health;

namespace WowGd.Src.Entities.Modes;

public partial class ModesManager : Node, IEntityHealthBinder
{
    /// <summary>
    /// </summary>
    /// <param name="mode"></param>
    /// <param name="asMain">
    /// default to true - if false, it allows to activate a mode in background of another, that is, without deactivating the current main active mode.
    /// </param>
    /// <returns>Whether the mode could be activated (and was correctly register under that manager, if asMain is true).</returns>
    public bool Activate(IMode mode, bool asMain = true)
    {
        if (asMain)
            return TrySelectActive(mode);

        return mode.Activate();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="mode"></param>
    /// <returns>Whether the provided mode could be deactivated.</returns>
    public bool Deactivate(IMode mode)
    {
        if (_active == mode)
            return UnselectActive();
        
        return mode.Deactivate();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns>Whether the main mode could be switched. That is, the current and next mode in list could respectively be deactivated and activated.</returns>
    public bool SwitchNext()
    {
        if (_modes.Count < 1)
            return false;

        if (_modes.Count == 1)
            return true;

        int index = (_index + 1) % _modes.Count;
        return SelectActive(_modes[index], index);
    }
}