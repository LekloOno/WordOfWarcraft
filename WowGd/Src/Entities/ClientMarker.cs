using Godot;
using WowGd.Src.Entities;
using WowGd.Src.Render;
using WowGd.Src.Tools;

[GlobalClass]
public partial class ClientMarker : Node
{
    public override void _Ready()
    {
        if (this.TryGetComposed(out IEntity? entity))
            ClientInfo.Entity = entity;
    }
}