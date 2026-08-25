namespace WowGd.Src.Dactylo.Worders;

public interface IWorderEraseHandler
{
    void OnEraseHit(char @char, int idx);
    void OnEraseMissed(char @char, int idx);
    void OnEraseMixed(char @char, int idx);
}