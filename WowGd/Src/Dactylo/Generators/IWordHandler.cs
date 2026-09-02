namespace WowGd.Src.Dactylo.Generators;

public interface IWordHandler
{
    void OnCharHit(char @char, int idx);
    void OnCharMissed(char @char, int idx);
    void OnCharMixed(char @char, int idx);
    
    void OnEraseHit(char @char, int idx);
    void OnEraseMissed(char @char, int idx);
    void OnEraseMixed(char @char, int idx);
    
    void OnEraseAllHit(int count, int idx);
    void OnEraseAllMissed(int count, int idx);
    void OnEraseAllMixed(int count, int idx);

    void OnCompleted();
}