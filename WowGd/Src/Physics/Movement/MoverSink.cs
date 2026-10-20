using WowGd.Src.Physics.Movement.Channels;
using WowGd.Src.Physics.Movement.Channels.Internal;
using WowGd.Src.Physics.Movement.Channels.Internal.Tackle;
using WowGd.Src.Physics.Movement.Status;
using WowGd.Src.Physics.Movement.WishDir;

namespace WowGd.Src.Physics.Movement;

public class MoverSink : IEntityMover
{
    public static readonly MoverSink Sink = new();

    private static readonly InternalChannel _internalChannel = new(new StandardWishDir());
    private static readonly DynamicTackleNode _dynamicTackleNode = new();
    private static readonly StatusQueryRegister _statusQueryRegister = new();
    private static readonly MovementChannels _movementChannels = new();

    public DynamicTackleNode DynamicTackleNode => _dynamicTackleNode;
    public StatusQueryRegister StatusChannels => _statusQueryRegister;
    public MovementChannels MovementChannels => _movementChannels;
    public InternalChannel Internal => _internalChannel;
    public ChannelSubResult AddContributor(MovementChannels channel, IContributor contributor, uint priority, bool strict = false) => ChannelSubResult.None;
    public ChannelSubResult RemoveContributor(MovementChannels channel, IContributor contributor) => ChannelSubResult.None;
}