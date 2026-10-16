using System;
using System.Collections.Generic;
using CombatCore;
using Godot;
using Godot.Collections;

namespace WowGd.Src.Combat.Statuses;

[GlobalClass]
public partial class StatusInstalls : Resource
{
    [Export] private Array<StatusInstall> _installs = [];

    internal Func<StatusInstance, Authority, IEnumerable<IDisposable>>? Build()
    {        
        return (inst, auth) => 
        {
            IDisposable[] installs = new IDisposable[_installs.Count];
        
            for (int i = 0; i < _installs.Count; i ++)
                installs[i] = _installs[i].Build(inst, auth);

            return installs;
        };
    }
}