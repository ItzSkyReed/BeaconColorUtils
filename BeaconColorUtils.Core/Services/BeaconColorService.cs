using BeaconColorUtils.Core.Enums;
using BeaconColorUtils.Core.Interfaces;
using BeaconColorUtils.Core.Models;
using BeaconColorUtils.Core.Processing;

namespace BeaconColorUtils.Core.Services;

public class BeaconColorService(IBeaconDataLoader dataLoader)
{
    private SequenceLut<long> _lut8Layers = null!;
    private SequenceLut<int> _lut7Layers = null!;
    private SequenceLut<int> _lut6Layers = null!;
    private OklabKdTree<int> _kdTree5Layers = null!;
    private OklabKdTree<int> _kdTree4Layers = null!;
    private OklabKdTree<short> _kdTree3Layers = null!;
    private OklabKdTree<short> _kdTree2Layers = null!;
    private OklabColor[] _cache1Layer = null!;


    private bool _isInitialized;


    public async Task InitializeAsync()
    {
        if (_isInitialized) return;

        var caches = await dataLoader.LoadCachesAsync();

        _lut8Layers = caches.Lut8;
        _lut7Layers = caches.Lut7;
        _lut6Layers = caches.Lut6;
        _kdTree5Layers = caches.KdTree5;
        _kdTree4Layers = caches.KdTree4;
        _kdTree3Layers = caches.KdTree3;
        _kdTree2Layers = caches.KdTree2;
        InitOneColorCache();

        _isInitialized = true;
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
    /// Used to find the closest sequence for 1 layer of glass
    /// </summary>
    private GlassColors[] FindClosestSequence(RgbColor targetColor)
    {
        if (!_isInitialized)
            throw new InvalidOperationException("Service not initialized");

        var targetOklab = OklabColor.FromRgb(targetColor);
        var minDelta = float.MaxValue;
        var bestIndex = -1;

        for (var i = 0; i < _cache1Layer.Length; i++)
        {
            var delta = OklabColor.DeltaE(targetOklab, _cache1Layer[i]);
            if (!(delta < minDelta)) continue;
            minDelta = delta;
            bestIndex = i;
        }

        // ReSharper disable once IntVariableOverflowInUncheckedContext
        return [(GlassColors)bestIndex];
    }

    public GlassColors[] GetBestMatch(RgbColor targetColor, int layersCount)
    {
        if (!_isInitialized)
            throw new InvalidOperationException("Service not initialized");

        return layersCount switch
        {
            1 => FindClosestSequence(targetColor),
            2 => new ColoredGlassSequence<short>(_kdTree2Layers.FindNearest(targetColor).Value).ToArray(),
            3 => new ColoredGlassSequence<short>(_kdTree3Layers.FindNearest(targetColor).Value).ToArray(),
            4 => new ColoredGlassSequence<int>(_kdTree4Layers.FindNearest(targetColor).Value).ToArray(),
            5 => new ColoredGlassSequence<int>(_kdTree5Layers.FindNearest(targetColor).Value).ToArray(),
            6 => new ColoredGlassSequence<int>(_lut6Layers.GetSequence(targetColor).Value).ToArray(),
            7 => new ColoredGlassSequence<int>(_lut7Layers.GetSequence(targetColor).Value).ToArray(),
            8 => new ColoredGlassSequence<long>(_lut8Layers.GetSequence(targetColor).Value).ToArray(),
            _ => throw new ArgumentOutOfRangeException(nameof(layersCount))
        };
    }
}