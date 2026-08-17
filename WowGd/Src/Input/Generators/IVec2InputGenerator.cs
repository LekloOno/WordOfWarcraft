using Godot;
using WowGd.Src.Tools;

namespace WowGd.Src.Input.Generators;

public interface IVec2InputGenerator : IDisablable
{
    public bool Retrieve(out Vector2 vec); 
}