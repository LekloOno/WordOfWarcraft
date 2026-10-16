using Godot;

namespace WowGd.Src.Combat;

public partial class CombatCoreLog : Node
{
    public override void _PhysicsProcess(double delta)
    {
        foreach(string line in CombatCoreNode.Authority.Log)
        {
            GD.Print(line);
        }

        CombatCoreNode.Authority.Log.Clear();
    }
}