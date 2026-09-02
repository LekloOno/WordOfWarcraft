using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Godot;

namespace WowGd.Src.Combat.Abilities.Actuation.Drivers;

/// <summary>
/// An overly simplified statistical typing AI.
/// </summary>
[GlobalClass]
public partial class BasicAiActuatorDriver : Node, IActuatorDriver
{   
    private float _wpm      = 100;
    private float _accuracy = 0.85f;
    private double _msPerStroke;

    [Export] public float Wpm
    {
        get => _wpm;
        set
        {
            if (value == _wpm)
                return;

            _wpm = value;
            UpdateMsPerStroke();
        }
    }

    [Export] public float Accuracy
    {
        get => _accuracy;
        set
        {
            if (value == _accuracy)
                return;
            
            _accuracy = value;
            UpdateMsPerStroke();
        }
    }

    private void UpdateMsPerStroke() =>
        _msPerStroke = 60_000.0 / (_wpm * 5.0);

    public override void _Ready()
    {
        UpdateMsPerStroke();
    }

    public async IAsyncEnumerable<TypingPackage> StreamTypingAsync(WordRequest request, [EnumeratorCancellation] CancellationToken ct)
    {
        try
        {
            int targetLength = (request.MinLength + request.MaxLength) / 2;

            while (!ct.IsCancellationRequested)
            {
                if (request.Mode == DeliveryMode.PerWord)
                    yield return await StartPerWord(targetLength, ct);
                else
                    yield return await StartPerKey(targetLength, ct);
            }

        }
        finally {}
    }

    private async Task<TypingPackage> StartPerKey(int targetLength, CancellationToken ct)
    {
        await Task.Delay(TimeSpan.FromMilliseconds(_msPerStroke), ct);

        bool correct = Random.Shared.NextSingle() < _accuracy;

        int correctKeys = correct ? 1 : 0;
        float acc = correctKeys;
        
        return new TypingPackage(correctKeys, acc, acc);
    }

    private async Task<TypingPackage> StartPerWord(int targetLength, CancellationToken ct)
    {
        await Task.Delay(TimeSpan.FromMilliseconds(_msPerStroke * targetLength), ct);

        int correctKeys = Mathf.FloorToInt(targetLength * _accuracy);

        return new(correctKeys, _accuracy, _accuracy);
    }
}