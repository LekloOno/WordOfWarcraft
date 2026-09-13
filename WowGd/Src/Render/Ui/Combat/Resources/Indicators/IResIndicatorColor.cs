using Godot;

namespace WowGd.Src.Render.Ui.Combat.Resources.Indicators;

public interface IResIndicatorColor
{
    Color ConsumedColor     { get; }
    Color GeneratedColor    { get; }
}