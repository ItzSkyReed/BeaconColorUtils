using System.Numerics;
using BeaconColorUtils.Core.Cache;
using BeaconColorUtils.Core.Enums;
using BeaconColorUtils.Core.Interfaces;
using BeaconColorUtils.Core.Models;
using BeaconColorUtils.Core.Processing;

namespace BeaconColorUtils.Core.Services;

public class BeaconColorService(IBeaconDataLoader dataLoader)
{
    private SequenceCache<ushort>? _cache3Layers;
    private SequenceCache<uint>? _cache4Layers;
    private SequenceCache<uint>? _cache5Layers;
    private SequenceCache<uint>? _cache6Layers;
    private OklabColor[]? _cache2Layers;
    private OklabColor[]? _cache1Layer;


    private bool _isInitialized;


    public async Task InitializeAsync()
    {
        if (_isInitialized) return;

        var caches = await dataLoader.LoadCachesAsync();

        _cache3Layers = caches.Cache3;
        _cache4Layers = caches.Cache4;
        _cache5Layers = caches.Cache5;
        _cache6Layers = caches.Cache6;

        InitTwoColorsCache();
        InitOneColorCache();

        _isInitialized = true;
    }

    private void InitTwoColorsCache()
    {
        var colors = Enum.GetValues<GlassColors>();
        var n = colors.Length;
        var count = n * n;

        _cache2Layers = new OklabColor[count];

        var index = 0;

        Span<GlassColors> sequence = stackalloc GlassColors[2];

        for (var i = 0; i < n; i++)
        {
            var c1 = colors[i];
            for (var j = 0; j < n; j++)
            {
                sequence[0] = c1;
                sequence[1] = colors[j];

                var blendedRgb = MinecraftBlender.Blend(sequence);
                _cache2Layers[index++] = OklabColor.FromRgb(blendedRgb);
            }
        }
    }

    private void InitOneColorCache()
    {
        var colors = Enum.GetValues<GlassColors>();
        var n = colors.Length;

        _cache1Layer = new OklabColor[n];

        Span<GlassColors> sequence = stackalloc GlassColors[1];

        for (var i = 0; i < n; i++)
        {
            sequence[0] = colors[i];

            var blendedRgb = MinecraftBlender.Blend(sequence);
            _cache1Layer[i] = OklabColor.FromRgb(blendedRgb);
        }
    }

    /// <summary>
    /// Used to find the closest sequence for 1/2 layers of glass
    /// </summary>
    private ColoredGlassSequence<T> FindClosestSequence<T>(RgbColor targetColor, int layers) where T : struct, IBinaryInteger<T>
    {
        if (!_isInitialized)
            throw new InvalidOperationException("Service not initialized");

        if (layers is < 1 or > 2)
            throw new ArgumentOutOfRangeException(nameof(layers), "Supports only 1 or 2 layers.");

        var targetOklab = OklabColor.FromRgb(targetColor);

        var minDelta = float.MaxValue;
        var bestIndex = -1;

        if (layers == 1)
        {
            for (var i = 0; i < _cache1Layer!.Length; i++)
            {
                var delta = OklabColor.DeltaE(targetOklab, _cache1Layer[i]);
                if (!(delta < minDelta)) continue;
                minDelta = delta;
                bestIndex = i;
            }

            Span<byte> sequence = stackalloc byte[1];
            // ReSharper disable once IntVariableOverflowInUncheckedContext
            sequence[0] = (byte)bestIndex;

            return new ColoredGlassSequence<T>(sequence);
        }
        else // layers == 2
        {
            for (var i = 0; i < _cache2Layers!.Length; i++)
            {
                var delta = OklabColor.DeltaE(targetOklab, _cache2Layers[i]);
                if (!(delta < minDelta)) continue;
                minDelta = delta;
                bestIndex = i;
            }

            var n = _cache1Layer?.Length ?? 16;
            var c1Index = bestIndex / n;
            var c2Index = bestIndex % n;

            if (c1Index == c2Index)
            {
                Span<byte> sequence1B = stackalloc byte[1];
                sequence1B[0] = (byte)c1Index;
                return new ColoredGlassSequence<T>(sequence1B);
            }

            Span<byte> sequence = stackalloc byte[2];
            sequence[0] = (byte)c1Index;
            sequence[1] = (byte)c2Index;

            return new ColoredGlassSequence<T>(sequence);
        }
    }

    public ColoredGlassSequence<T> GetBestMatch<T>(RgbColor targetColor, int layersCount) where T : struct, IBinaryInteger<T>
    {
        if (!_isInitialized)
            throw new InvalidOperationException("Service not initialized");

        return layersCount switch
        {
            1 or 2 => FindClosestSequence<T>(targetColor, layersCount),
            3 => new ColoredGlassSequence<T>(T.CreateTruncating(_cache3Layers!.GetSequence(targetColor).Value)),
            4 => new ColoredGlassSequence<T>(T.CreateTruncating(_cache4Layers!.GetSequence(targetColor).Value)),
            5 => new ColoredGlassSequence<T>(T.CreateTruncating(_cache5Layers!.GetSequence(targetColor).Value)),
            6 => new ColoredGlassSequence<T>(T.CreateTruncating(_cache6Layers!.GetSequence(targetColor).Value)),
            _ => throw new ArgumentOutOfRangeException(nameof(layersCount))
        };
    }
}