namespace WowGd.Src.Dactylo.Generators;

public static class WordBinder
{
    public static void Bind(this IWordHandler handler, Word word)
    {
        word.CharHit        += handler.OnCharHit;
        word.CharMissed     += handler.OnCharMissed;
        word.CharMixed      += handler.OnCharMixed;

        
        word.EraseHit       += handler.OnEraseHit;
        word.EraseMissed    += handler.OnEraseMissed;
        word.EraseMixed     += handler.OnEraseMixed;

        word.EraseAllHit    += handler.OnEraseAllHit;
        word.EraseAllMissed += handler.OnEraseAllMissed;
        word.EraseAllMixed  += handler.OnEraseAllMixed;

        word.Completed      += handler.OnCompleted;
    }

    public static void Unbind(this IWordHandler handler, Word word)
    {
        word.CharHit        -= handler.OnCharHit;
        word.CharMissed     -= handler.OnCharMissed;
        word.CharMixed      -= handler.OnCharMixed;

        word.EraseHit       -= handler.OnEraseHit;
        word.EraseMissed    -= handler.OnEraseMissed;
        word.EraseMixed     -= handler.OnEraseMixed;

        word.EraseAllHit    -= handler.OnEraseAllHit;
        word.EraseAllMissed -= handler.OnEraseAllMissed;
        word.EraseAllMixed  -= handler.OnEraseAllMixed;
        
        word.Completed      -= handler.OnCompleted;
    }
}