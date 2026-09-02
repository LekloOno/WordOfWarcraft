using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Godot;

namespace WowGd.Src.Input;

public static class AbilitySelectInputExt
{
    public const string AbilitySelect0 = "ability_select_0";
    public const string AbilitySelect1 = "ability_select_1";
    public const string AbilitySelect2 = "ability_select_2";
    public const string AbilitySelect3 = "ability_select_3";
    public const string AbilitySelect4 = "ability_select_4";
    public const string AbilitySelect5 = "ability_select_5";
    public const string AbilitySelect6 = "ability_select_6";
    public const string AbilitySelect7 = "ability_select_7";
    public const string AbilitySelect8 = "ability_select_8";
    public const string AbilitySelect9 = "ability_select_9";

    public const int MaxIndex = 9;

    public static bool TryGetAbilityIndex(this InputEvent @event, out int index)
    {
        if (@event.IsActionPressed(AbilitySelect0))
            index = 0;
        else if (@event.IsActionPressed(AbilitySelect1))
            index = 1;
        else if (@event.IsActionPressed(AbilitySelect2))
            index = 2;
        else if (@event.IsActionPressed(AbilitySelect3))
            index = 3;
        else if (@event.IsActionPressed(AbilitySelect4))
            index = 4;
        else if (@event.IsActionPressed(AbilitySelect5))
            index = 5;
        else if (@event.IsActionPressed(AbilitySelect6))
            index = 6;
        else if (@event.IsActionPressed(AbilitySelect7))
            index = 7;
        else if (@event.IsActionPressed(AbilitySelect8))
            index = 8;
        else if (@event.IsActionPressed(AbilitySelect9))
            index = 9;
        else
            index = -1;

        return index != -1;
    }

    private static string GetAbilityActionName(this int index) => index switch
    {
        0 => AbilitySelect0,
        1 => AbilitySelect1,
        2 => AbilitySelect2,
        3 => AbilitySelect3,
        4 => AbilitySelect4,
        5 => AbilitySelect5,
        6 => AbilitySelect6,
        7 => AbilitySelect7,
        8 => AbilitySelect8,
        9 => AbilitySelect9,
        _ => throw new ArgumentOutOfRangeException(),
    };

    public static int TryGetAbilityInputs(this int index, [NotNullWhen(true)] out ICollection<InputEvent> inputs)
    {
        if (index < 0 || index > MaxIndex)
        {
            inputs = [];
            return 0;
        }

        string name = index.GetAbilityActionName();
        inputs = InputMap.ActionGetEvents(name);
        return inputs.Count;
    }

    public static bool TryGetAbilityFirstKey(this int index, out string key)
    {
        if (index < 0 || index > MaxIndex)
        {
            key = string.Empty;
            return false;
        }

        string name = index.GetAbilityActionName();
        foreach (InputEvent inputEvent in InputMap.ActionGetEvents(name))
        {
            if (inputEvent is InputEventKey keyEvent)
            {
                key = char.ConvertFromUtf32((int)keyEvent.Unicode);
                return true;
            }
        }

        key = string.Empty;
        return false;
    }
}