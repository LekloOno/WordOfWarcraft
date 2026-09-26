using System.Numerics;

namespace WowGd.Src.Tools;

public ref struct SetBitEnumerator(uint bits)
{
    private uint _bits = bits;
    public int Current { get; private set; } = 0;

    public bool MoveNext()
    {
        if (_bits == 0)
            return false;

        Current = BitOperations.TrailingZeroCount(_bits);
        _bits &= _bits - 1;

        return true;
    }

    public readonly SetBitEnumerator GetEnumerator() => this;
}

public static class BitFlags
{
    public static SetBitEnumerator Enumerate(uint bits)
        => new(bits);
}