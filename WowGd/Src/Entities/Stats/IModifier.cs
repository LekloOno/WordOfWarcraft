namespace WowGd.Src.Entities.Stats;

public interface IModifier<T>
{
    T Apply(T value);
}