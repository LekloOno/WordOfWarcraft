namespace WowGd.Src.Combat.Abilities.Exp.Data.Resources;

public interface IComponentFactory<out TComponent>
{
    TComponent Build();
}