namespace WowGd.Src.Combat.Resources.FocusRes;

public interface IStandardResourceHandler
{
    void OnConsumed(int fp);
    void OnGenerated(int fp);
}