namespace WowGd.Src.Combat.Resources;

public interface IStandardResourceHandler
{
    void OnConsumed(int fp);
    void OnGenerated(int fp);
    void OnMaxChanged(int max) {}
}