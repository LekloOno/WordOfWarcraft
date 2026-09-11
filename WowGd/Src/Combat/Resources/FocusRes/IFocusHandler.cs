namespace WowGd.Src.Combat.Resources.FocusRes;

public interface IFocusHandler
{
    void OnConsumed(int fp);
    void OnGenerated(int fp);
}