using Godot;
using System.Collections.Generic;
using WowGd.Src.Combat.Health;

namespace WowGd.Src.Entities.Modes;

[GlobalClass]
public partial class ModesManager : Node, IEntityHealthBinder
{
    [Export] private bool _initActivate = true;

    private readonly List<IMode> _modes = [];
    private readonly Dictionary<IMode, int> _indexes = [];

    private IMode? _active;
    private int _index = -1;

    public override void _Ready()
    {
        foreach (Node node in GetChildren())
            if (node is IMode mode)
            {
                _indexes[mode] = _modes.Count;
                _modes.Add(mode);
            }

        if (_initActivate && _modes.Count > 0)
            SelectActive(_modes[0], 0);
    }

    public void Bind(IEntityHealth health) =>
        this.BindChildren(health);

    public void Unbind(IEntityHealth health) =>
        this.UnbindChildren(health);

    private bool TrySelectActive(IMode mode)
    {
        if (!_indexes.TryGetValue(mode, out int index))
            return false;

        return SelectActive(mode, index);
    }

    private bool SelectActive(IMode mode, int index)
    {
        if (_active == mode)
            return true;

        if (!UnselectActive())
            return false;

        if (!mode.Activate())
            return false;

        _active = mode;
        _index  = index;
        return true;
    }

    private bool UnselectActive()
    {
        if (_active?.Deactivate() is false)
            return false;

        _active = null;
        _index  = -1;
        return true;
    }
}