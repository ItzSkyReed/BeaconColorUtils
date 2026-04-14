using System.Numerics;
using BeaconColorUtils.Core.Models;

namespace BeaconColorUtils.Core.Cache;

/// <summary>
/// O(1) in-memory glass sequence storage.
/// </summary>
public class SequenceCache<T> where T : unmanaged, IBinaryInteger<T>
{
    public const int CacheSize = 16_777_216;

    private readonly T[] _cache;

    public SequenceCache(T[] preloadedData)
    {
        if (preloadedData.Length != CacheSize)
            throw new ArgumentException($"Cache size must be {CacheSize} elements.");

        _cache = preloadedData;
    }

    public ColoredGlassSequence<T> GetSequence(RgbColor color)
    {
        var index = (color.R << 16) | (color.G << 8) | color.B;
        return new ColoredGlassSequence<T>(_cache[index]);
    }

    public ColoredGlassSequence<T> GetSequence(byte r, byte g, byte b)
    {
        var index = (r << 16) | (g << 8) | b;
        return new ColoredGlassSequence<T>(_cache[index]);
    }
}