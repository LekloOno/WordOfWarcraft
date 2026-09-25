using System.Diagnostics.CodeAnalysis;
using Godot;

namespace WowGd.Src.Tools;

public static class StaticTree
{
    public static bool TryGetTree([NotNullWhen(true)] out SceneTree? tree)
    {
        if (Engine.GetMainLoop() is SceneTree sceneTree)
        {
            tree = sceneTree;
            return true;
        }

        tree = null;
        return false;
    }
}