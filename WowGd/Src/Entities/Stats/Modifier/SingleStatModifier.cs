using System;
using Godot;

namespace WowGd.Src.Entities.Stats.Modifier;

[GlobalClass, Tool]
public partial class SingleStatModifier : StatModifier
{
    [Export] public StatEnum Target;
    [Export] public bool Flat
    {
        get => _flat;
        set
        {
            if (_flat == value)
                return;

            _flat = value;
            NotifyPropertyListChanged();
        }
    }
    [Export] public int     Value       { get; private set; } = 0;
    [Export] public float   Multiplier  { get; private set; } = 1f;
    [Export] private int    _layer;

    private bool _flat = true;

    public override IDisposable? ApplyTo(IEntity entity, float weight) => _flat
        ? Target.GetStat(entity).Buckets.AddFlatModifier(Mathf.FloorToInt(Value * weight))
        : Target.GetStat(entity).Buckets.AddLayerModifier(Multiplier * weight, _layer);

    public override void _ValidateProperty(Godot.Collections.Dictionary property)
    {
        string name = property["name"].AsString();

        if (Flat)
        {
            if (name == nameof(Multiplier) || name == nameof(_layer))
                property["usage"] = (uint)PropertyUsageFlags.NoEditor;
        }
        else
        {
            if (name == nameof(Value))
                property["usage"] = (uint)PropertyUsageFlags.NoEditor;
        }
    }
}