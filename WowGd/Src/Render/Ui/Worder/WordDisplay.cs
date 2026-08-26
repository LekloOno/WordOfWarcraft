using Godot;
using WowGd.Src.Dactylo.Generators;

namespace WowGd.Src.Render.Ui;

public partial class WordDisplay : RichTextLabel
{
    public WordDisplay(Word word)
    {
        _word = word;
        FitContent = true;
        BbcodeEnabled = true;
        AutowrapMode = TextServer.AutowrapMode.Off;
    }
    private const string HolderColor = "#666666";
    private const string CorrectColor = "#EEEEEE";
    private const string MistakeColor = "#E57373";
    private const string OverflowColor = "#C94C4C";

    private readonly Word _word;

    public void UpdateDisplay()
    {
        string output = "";

        for (int i = 0; i < _word.Content.Length; i++)
        {
            if (i >= _word.Written.Length)
            {
                output += FormatCharacter(
                    _word.Content[i].ToString(),
                    HolderColor
                );
            }
            else if (_word.Written[i] == _word.Content[i])
            {
                output += FormatCharacter(
                    _word.Written[i].ToString(),
                    CorrectColor
                );
            }
            else
            {
                output += FormatCharacter(
                    _word.Written[i].ToString(),
                    MistakeColor
                );
            }
        }

        if (_word.Written.Length > _word.Content.Length)
        {
            for (int i = _word.Content.Length; i < _word.Written.Length; i++)
            {
                output += FormatCharacter(
                    _word.Written[i].ToString(),
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
            _word.Written,
            HorizontalAlignment.Left,
            -1,
            fontSize
        ).X;

        return GlobalPosition + Vector2.Right * width;
    }
}