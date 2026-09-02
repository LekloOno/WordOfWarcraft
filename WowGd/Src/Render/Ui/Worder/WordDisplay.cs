using Godot;
using WowGd.Src.Dactylo.Generators;

namespace WowGd.Src.Render.Ui;

public partial class WordDisplay : RichTextLabel, IWordHandler
{
    public WordDisplay(Word word)
    {
        Word = word;
        FitContent = true;
        BbcodeEnabled = true;
        AutowrapMode = TextServer.AutowrapMode.Off;

        this.Bind(word);
    }
    private const string HolderColor = "#666666";
    private const string CorrectColor = "#EEEEEE";
    private const string MistakeColor = "#E57373";
    private const string OverflowColor = "#C94C4C";

    public readonly Word Word;

    public void UpdateDisplay()
    {
        string output = "";

        for (int i = 0; i < Word.Content.Length; i++)
        {
            if (i >= Word.Written.Length)
            {
                output += FormatCharacter(
                    Word.Content[i].ToString(),
                    HolderColor
                );
            }
            else if (Word.Written[i] == Word.Content[i])
            {
                output += FormatCharacter(
                    Word.Written[i].ToString(),
                    CorrectColor
                );
            }
            else
            {
                output += FormatCharacter(
                    Word.Written[i].ToString(),
                    MistakeColor
                );
            }
        }

        if (Word.Written.Length > Word.Content.Length)
        {
            for (int i = Word.Content.Length; i < Word.Written.Length; i++)
            {
                output += FormatCharacter(
                    Word.Written[i].ToString(),
                    OverflowColor
                );
            }
        }

        Text = output;
    }

    private static string FormatCharacter(string character, string color)
    {
        return $"[color={color}]{character}[/color]";
    }

    public Vector2 GetCaretPosition()
    {
        Font font = GetThemeFont("normal_font");
        int fontSize = GetThemeFontSize("normal_font_size");

        float width = font.GetStringSize(
            Word.Written,
            HorizontalAlignment.Left,
            -1,
            fontSize
        ).X;

        return GlobalPosition + Vector2.Right * width;
    }

    public void OnCharHit(char @char, int idx) =>
        UpdateDisplay();
    public void OnCharMissed(char @char, int idx) =>
        UpdateDisplay();
    public void OnCharMixed(char @char, int idx) =>
        UpdateDisplay();
    public void OnEraseHit(char @char, int idx) =>
        UpdateDisplay();
    public void OnEraseMissed(char @char, int idx) =>
        UpdateDisplay();
    public void OnEraseMixed(char @char, int idx) =>
        UpdateDisplay();
    public void OnEraseAllHit(int count, int idx) =>
        UpdateDisplay();
    public void OnEraseAllMissed(int count, int idx) =>
        UpdateDisplay();
    public void OnEraseAllMixed(int count, int idx) =>
        UpdateDisplay();

    public void OnCompleted()
    {
        this.Unbind(Word);
    }
}