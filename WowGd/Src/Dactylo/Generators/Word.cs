using System.Text;

namespace WowGd.Src.Dactylo.Generators;

public enum WordKeyOutcome
{
    /// <summary>
    /// Write - Wrong character typed.
    /// Erase - Unnecessarily erased in a fully correct word.
    /// </summary>
    Miss,
    /// <summary>
    /// Write - Right character right in a correct word.
    /// Erase - Erased an incorrect character.
    /// </summary>
    Hit,
    /// <summary>
    /// Write - Right character written in an incorrect word.
    /// Erase - Either :
    ///     1. The removed character was not a mistake, but there's a mistake earlier.
    ///     2. Multiple keys have been removed at once, including both misstakes and correct ones.
    /// </summary>
    Mixed,
    /// <summary>
    /// Interraction ignored. No effect on the word.
    /// </summary>
    Ignored,
}

public class Word(string content)
{
    private readonly string _content = content;
    public readonly int Length = content.Length;
    public string Content => _content;
    public string Written => _written.ToString();
    public int WrittenLength => _written.Length;

    private readonly StringBuilder _written = new(content.Length);
    public int Idx      {get; private set;} = -1;
    public int Correct  {get; private set;} = 0;


    public WordKeyOutcome Write(char @char)
    {
        _written.Append(@char);
        
        Idx ++;
        bool correct = Idx < Length && @char == _content[Idx];
        
        
        if (!correct)
            return WordKeyOutcome.Miss;

        Correct ++;
        if (Correct == Idx)
            return WordKeyOutcome.Hit;
        return WordKeyOutcome.Mixed;
    }

    public WordKeyOutcome EraseOne(out char @char)
    {
        if (Idx == -1)
        {
            @char = ' ';
            return WordKeyOutcome.Ignored;
        }

        @char = _written[Idx];
        bool hit = Idx < Length && @char != _content[Idx];

        _written.Remove(Idx, 1);
        Idx --;
        
        if (hit)
            return WordKeyOutcome.Hit;
        
        Correct --;
        // If the removed char was not a mistake
        //  and no remaining char is a mistake
        //  there were no reason to remove that char.
        if (Correct == Idx)
            return WordKeyOutcome.Miss;
        // Otherwise, it can be a decision
        //  to fix an earlier mistake
        return WordKeyOutcome.Mixed;
    }

    public WordKeyOutcome EraseAll()
    {
        _written.Clear();

        int cachedIdx = Idx;
        int cachedCorrect = Correct;
        Idx = -1;
        Correct = 0;

        if (cachedCorrect == 0)
            return WordKeyOutcome.Hit;
        
        if (cachedCorrect == cachedIdx)
            return WordKeyOutcome.Miss;

        return WordKeyOutcome.Mixed;
    }
}