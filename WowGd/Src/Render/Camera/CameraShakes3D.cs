using Godot;
using WowGd.Src.Render.Animation.Shakes;

namespace WowGd.Src.Render.Camera;

[GlobalClass]
public partial class CameraShakes3D : Node3D, ITraumaReceiver, ITraumaChannel
{
    [Export] private TraumaChannelSettings _baseChannelSettings = null!;
    private ReceiverShakeOutput? _receiverShakeOutput;

    public ITraumaChannel BaseChannel => _receiverShakeOutput!.BaseChannel;

    public float Intensity => _receiverShakeOutput!.Intensity;
    public bool Active => _receiverShakeOutput!.Active;
    public void Add(float amount) => _receiverShakeOutput!.Add(amount);
    public void RaiseTo(float level) => _receiverShakeOutput!.RaiseTo(level);
    
    public ITraumaChannel? OpenChannel(ITraumaSettings settings, ITraumaSamplerSettings samplerSettings) =>
        _receiverShakeOutput?.OpenChannel(settings, samplerSettings);

    public override void _EnterTree()
    {
        if (_receiverShakeOutput != null)
            return;

        _receiverShakeOutput = new(_baseChannelSettings, _baseChannelSettings);
        _receiverShakeOutput.Shaken += OnShaken;
        AddChild(_receiverShakeOutput);
    }

    private void OnShaken(Vector3 vector)
    {
        Rotation = vector;
        //GD.Print(vector);
    }
}