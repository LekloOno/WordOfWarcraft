namespace WowGd.Src.Render.Animation.Shakes;

public interface ITraumaReceiver
{
    ITraumaChannel BaseChannel { get; }
    ITraumaChannel? OpenChannel(ITraumaSettings settings, ITraumaSamplerSettings samplerSettings);
}