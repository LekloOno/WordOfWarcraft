namespace WowGd.Src.Dactylo.Worders;

public interface IWorderCharHandler
{
    void OnCharHit(char @char, int idx);
    void OnCharMissed(char @char, int idx);
    void OnCharMixed(char @char, int idx);
}