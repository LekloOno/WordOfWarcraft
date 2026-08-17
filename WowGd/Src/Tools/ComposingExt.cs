using System.Diagnostics.CodeAnalysis;
using Godot;

namespace WowGd.Src.Tools;

public static class ComposingExt
{
    public static bool TryGetComposed<T>(this Node self, [NotNullWhen(true)] out T? composed)
    {
        composed = default;
        if (self.GetParent() is not T p)
        {
            GD.PushError($"[{self.GetType()}] requires a [{nameof(T)}] parent.");
            return false;
        }

        composed = p;
        return true;
    }

    public static bool TryGetComponent<T>(this Node self, [NotNullWhen(true)] out T? component)
    {
        component = default;
        
        foreach (Node child in self.GetChildren())
        {
            if (child is not T c)
                continue;
            
            component = c;
            return true;
        }

        GD.PushError($"[{self.GetType()}] requires a [{nameof(T)}] child.");
        return false;
    }

    public static T CreateComponent<T>(this Node self)
    where
        T : Node, new()
    {
        T component = new();
        self.AddChild(component);
        return component;    
    }
}