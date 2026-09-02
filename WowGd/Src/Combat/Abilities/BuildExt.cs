using System.Collections.Generic;
using WowGd.Src.Combat.Abilities.Launch;

namespace WowGd.Src.Combat.Abilities;

public static class BuildExt
{
    public static ILaunch[] BuildAll(this ICollection<LaunchData> data)
    {
        int count = data.Count;

        ILaunch[] launches = new ILaunch[count];
        
        int i = 0;
        foreach (LaunchData datum in data)
        {
            launches[i] = datum.Build();
            i ++;
        }

        return launches;
    }
}