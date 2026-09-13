namespace WowGd.Src.Render.Ui.Combat.Resources.Indicators;

public interface IResIndicatorRelationColors<T>
    where T : IResIndicatorColor
{
    T Self    { get; }
    T Ally    { get; }
    T Enemy   { get; }
}