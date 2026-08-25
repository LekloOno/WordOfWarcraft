using Godot;
using WowGd.Src.Dactylo.Generators;
using WowGd.Src.Dactylo.Worders;

namespace WowGd.Src.Render.Ui;

public partial class WorderDisplay : HFlowContainer, IWorderHandler
{
    [Export] private Worder _worder = null!;
    private WordDisplay? _current;
    private int _idx = 0;

    public override void _Ready()
    {
        this.Bind(_worder);
    }

    public void OnCharHit(char @char, int idx)
    {
        _current?.UpdateDisplay();
    }

    public void OnCharMissed(char @char, int idx)
    {
        _current?.UpdateDisplay();
    }

    public void OnCharMixed(char @char, int idx)
    {
        _current?.UpdateDisplay();
    }

    public void OnCompleted(string written, string target, int correct)
    {
        _current?.UpdateDisplay();
    }

    public void OnEraseAllHit(int count, int idx)
    {
        _current?.UpdateDisplay();
    }

    public void OnEraseAllMissed(int count, int idx)
    {
        _current?.UpdateDisplay();
    }

    public void OnEraseAllMixed(int count, int idx)
    {
        _current?.UpdateDisplay();
    }

    public void OnEraseHit(char @char, int idx)
    {
        _current?.UpdateDisplay();
    }

    public void OnEraseMissed(char @char, int idx)
    {
        _current?.UpdateDisplay();
    }

    public void OnEraseMixed(char @char, int idx)
    {
        _current?.UpdateDisplay();
    }

    public void OnWordEnqueued(Word word)
    { 
        WordDisplay wd = new(word);
        wd.UpdateDisplay();
        AddChild(wd);
    }

    public void OnWordStarted(string word, int remainingWords)
    {
        _current = GetChild(_idx) as WordDisplay;
        _idx ++;
    }
}