using System.Collections;
using BeaconColorUtils.Core.Enums;
using BeaconColorUtils.Core.Processing;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace BeaconColorUtils.Core.Models;

public readonly struct ColoredGlassSequence<T> : IReadOnlyList<GlassColors> where T : struct, IBinaryInteger<T>
{
    public readonly T Value;

    public ColoredGlassSequence(T value)
    {
        Value = value;
    }

    public IEnumerator<GlassColors> GetEnumerator()
    {
        var len = Count;
        for (var i = 0; i < len; i++)
        {
            yield return this[i];
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public GlassColors this[int index]
    {
        get
        {
            var shift = 3 + index * 4;

            var masked = (Value >>> shift) & T.CreateTruncating(0b1111);

            return (GlassColors)byte.CreateTruncating(masked);
        }
    }

    public ColoredGlassSequence(ReadOnlySpan<byte> colors)
    {
        var maxColors = Unsafe.SizeOf<T>() == 2 ? 3 : 6;

        if (colors.Length < 1 || colors.Length > maxColors)
            throw new ArgumentException($"For {typeof(T).Name}, length must be from 1 to {maxColors}");

        var packedValue = T.CreateTruncating(colors.Length);

        for (var i = 0; i < colors.Length; i++)
        {
            var colorVal = T.CreateTruncating(colors[i]);
            packedValue |= colorVal << (3 + i * 4);
        }

        Value = packedValue;
    }

    public int Count => int.CreateTruncating(Value & T.CreateTruncating(0b111));

    /// <summary>
    /// Returns a mapping dictionary: Color ID -> Color name (For example: 0 -> "White").
    /// </summary>
    public static Dictionary<byte, string> GetColorMappings()
    {
        var map = new Dictionary<byte, string>(16);
        for (byte i = 0; i < MinecraftBlender.ColorNames.Length; i++)
        {
            map[i] = MinecraftBlender.ColorNames[i];
        }

        return map;
    }

    /// <summary>
    /// Returns an array of color names for the current glass sequence.
    /// </summary>
    public string[] GetColorNames()
    {
        var len = Count;
        var names = new string[len];
        for (var i = 0; i < len; i++)
        {
            names[i] = MinecraftBlender.ColorNames[(byte)this[i]];
        }

        return names;
    }

    /// <summary>
    /// "Red -> Orange -> Yellow"
    /// </summary>
    public override string ToString() => string.Join(" -> ", GetColorNames());

    public GlassColors[] ToArray()
    {
        var len = Count;
        var arr = new GlassColors[len];
        for (var i = 0; i < len; i++)
        {
            arr[i] = this[i];
        }

        return arr;
    }
}