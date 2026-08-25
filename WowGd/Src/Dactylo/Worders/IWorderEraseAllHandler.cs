namespace WowGd.Src.Dactylo.Worders;

public interface IWorderEraseAllHandler
{
    void OnEraseAllHit(int count, int idx);
    void OnEraseAllMissed(int count, int idx);
    void OnEraseAllMixed(int count, int idx);
}