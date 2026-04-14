using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using BeaconColorUtils.Core.Models;
using BeaconColorUtils.Core.Processing;
using CommunityToolkit.Mvvm.ComponentModel;
using BeaconColorUtils.Core.Services;
using BeaconColorUtils.UI.Models;
using CommunityToolkit.Mvvm.Input;

namespace BeaconColorUtils.UI.ViewModels;

public partial class BestGlassSequenceViewModel : ViewModelBase
{
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(TargetBrush))]
    [NotifyPropertyChangedFor(nameof(TargetColorHex))]
    public partial Color TargetColor { get; set; } = Random.Shared.Next(3) switch
    {
        0 => Color.FromRgb(255, (byte)Random.Shared.Next(256), (byte)Random.Shared.Next(256)),
        1 => Color.FromRgb((byte)Random.Shared.Next(256), 255, (byte)Random.Shared.Next(256)),
        _ => Color.FromRgb((byte)Random.Shared.Next(256), (byte)Random.Shared.Next(256), 255)
    };

    [ObservableProperty]
    public partial int MaxLayers { get; set; } = 6;

    private Color _previousColor;
    private int _previousMaxLayers;

    [ObservableProperty]
    public partial string HexInputText { get; set; }

    public string TargetColorHex
    {
        get => $"{TargetColor.R:X2}{TargetColor.G:X2}{TargetColor.B:X2}";
        set
        {
            if (string.IsNullOrWhiteSpace(value)) return;

            var cleanHex = value.Replace("#", "").Trim();

            if (cleanHex.Length is 6 or 8 && Color.TryParse($"#{cleanHex}", out var parsedColor))
            {
                TargetColor = parsedColor;
            }
        }
    }

    public SolidColorBrush TargetBrush => new(TargetColor);

    public Bitmap? BaseBeaconImage { get; }

    public ObservableCollection<CalculationResult> Results { get; } = [];

    private readonly BeaconColorService _colorService;

    public BestGlassSequenceViewModel(BeaconColorService colorService)
    {
        _colorService = colorService;

        // Beacon beam loading
        var uri = new Uri("avares://BeaconColorUtils.UI/Assets/Beacon_Beam.png");
        using var stream = AssetLoader.Open(uri);
        BaseBeaconImage = new Bitmap(stream);

        HexInputText = $"{TargetColor.R:X2}{TargetColor.G:X2}{TargetColor.B:X2}";
    }

    partial void OnTargetColorChanged(Color value)
    {
        HexInputText = $"{value.R:X2}{value.G:X2}{value.B:X2}";
    }

    partial void OnHexInputTextChanged(string value)
    {
        if (string.IsNullOrWhiteSpace(value)) return;

        var cleanHex = value.Replace("#", "").Trim();

        if (cleanHex.Length is 6 or 8 && Color.TryParse($"#{cleanHex}", out var parsedColor))
        {
            TargetColor = parsedColor;
        }
    }

    [RelayCommand]
    private void ValidateAndRevertHex()
    {
        HexInputText = $"{TargetColor.R:X2}{TargetColor.G:X2}{TargetColor.B:X2}";
    }

    private CalculationResult GetResultTemplate(int layersCount)
    {
        var targetColor = new RgbColor(TargetColor.R, TargetColor.G, TargetColor.B);
        var color = _colorService.GetBestMatch<int>(targetColor, layersCount);

        var resultRgb = MinecraftBlender.Blend(color.ToArray());

        var deltaE = OklabColor.DeltaE(OklabColor.FromRgb(targetColor), OklabColor.FromRgb(resultRgb));

        var accuracy = OklabColor.GetSimilarityScore(deltaE);

        var glasses = new List<GlassPane>(layersCount);
        glasses.AddRange(color.ToArray().Select(GlassPane.FromColor));


        return new CalculationResult(accuracy, deltaE, resultRgb.toHexString(), glasses);
    }

    [RelayCommand]
    private void Calculate()
    {
        if (_previousColor == TargetColor && _previousMaxLayers == MaxLayers)
            return;

        _previousColor = TargetColor;
        _previousMaxLayers = MaxLayers;

        var sortedUniqueResults = Enumerable.Range(1, MaxLayers)
            .Select(GetResultTemplate)
            .Distinct()
            .OrderByDescending(r => r.Accuracy)
            .ToList();

        Results.Clear();
        foreach (var result in sortedUniqueResults)
        {
            Results.Add(result);
        }
    }
}