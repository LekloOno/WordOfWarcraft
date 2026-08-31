using System.Collections.Generic;
using Godot;
using WowGd.Src.Combat.Abilities.Exp.Launch;

namespace WowGd.Src.Combat.Abilities.Exp;

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

    public static ILaunch[] SyncAll(this ICollection<LaunchData> data, Node parent)
    {
        int count = data.Count;

        ILaunch[] launches = new ILaunch[count];
        
        int i = 0;
        foreach (LaunchData datum in data)
        {
            ILaunch launch = datum.Build();
            launches[i] = launch;

            if (launch is Node node)
                parent.AddChild(node);

            i ++;
        }

        return launches;
    }
}