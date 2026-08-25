using System;

namespace WowGd.Src.Tools;

public static class DisableExt
{
    public static bool IndempEnDis(ref bool enabled, Action func, bool enable)
    {
        if (enabled == enable)
            return false;

        enabled = enable;
        func();
        return true;    
    }

    public static bool IndempEnable(ref bool enabled, Action func) => IndempEnDis(ref enabled, func, true);
    public static bool IndempDisable(ref bool enabled, Action func) => IndempEnDis(ref enabled, func, false);

    public static bool IndempActDea(ref bool active, bool enabled, Action func, bool activated)
    {
        if (!enabled)
            return false;

        return IndempEnDis(ref active, func, activated);
    }

    public static bool IndempActivate(ref bool active, bool enabled, Action func) => IndempActDea(ref active, enabled, func, true);
    public static bool IndempDeactivate(ref bool active, bool enabled, Action func) => IndempActDea(ref active, enabled, func, false);

    public static bool IndempEnableActivable(ref bool enabled, bool active, Action func)
    {
        if (enabled)
            return false;

        enabled = true;
        
        if (active)
            func();

        return true; 
    }
}