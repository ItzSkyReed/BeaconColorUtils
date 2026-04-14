using BeaconColorUtils.Core.Enums;
using BeaconColorUtils.Core.Processing;

namespace BeaconColorUtils.Core.Models;

using System;
using System.Numerics;
using System.Runtime.CompilerServices;

public readonly struct ColoredGlassSequence<T> where T : struct, IBinaryInteger<T>
{
    public readonly T Value;

    public ColoredGlassSequence(T value)
    {
        Value = value;
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

    public int Length => int.CreateTruncating(Value & T.CreateTruncating(0b111));

    public byte GetColor(int index)
    {
        var shift = 3 + index * 4;

        var masked = (Value >>> shift) & T.CreateTruncating(0b1111);

        return byte.CreateTruncating(masked);
    }

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
        var len = Length;
        var names = new string[len];
        for (var i = 0; i < len; i++)
        {
            names[i] = MinecraftBlender.ColorNames[GetColor(i)];
        }
        return names;
    }

    /// <summary>
    /// "Red -> Orange -> Yellow"
    /// </summary>
    public override string ToString() => string.Join(" -> ", GetColorNames());

    public GlassColors[] ToArray()
    {
        var len = Length;
        var arr = new GlassColors[len];
        for (var i = 0; i < len; i++)
        {
            arr[i] = (GlassColors)GetColor(i);
        }
        return arr;
    }
}