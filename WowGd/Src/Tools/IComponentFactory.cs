namespace WowGd.Src.Tools;

public interface IComponentFactory<out TComponent>
{
    TComponent Build();
}