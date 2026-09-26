using WowGd.Src.Physics.Movement.Channels;
using WowGd.Src.Physics.Movement.Channels.Internal;
using WowGd.Src.Physics.Movement.Status;

namespace WowGd.Src.Physics.Movement;

public interface IEntityMover
{
    StatusQueryRegister StatusChannels { get; }
    MovementChannels MovementChannels { get; }
    InternalChannel Internal { get; }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="channel"></param>
    /// <param name="contributor"></param>
    /// <param name="priority"></param>
    /// <param name="strict">Whether the contributor should only be added when the target channel is already available.</param>
    /// <returns>The result of the operation, as packed flags.</returns>
    ChannelSubResult AddContributor(MovementChannels channel, IContributor contributor, uint priority, bool strict = false);
    ChannelSubResult RemoveContributor(MovementChannels channel, IContributor contributor);
}