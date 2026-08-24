using WowGd.Src.Combat.Health;
using WowGd.Src.Physics;

namespace WowGd.Src.Entities;

public interface IEntity
{
    public uint TeamMask        { get; }
    public Body Body            { get; }
    public IEntityHealth Health { get; }
}