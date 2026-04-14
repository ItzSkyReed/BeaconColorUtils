using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Media;
using Avalonia.Platform.Storage;
using Avalonia.Threading;
using BeaconColorUtils.Core.Enums;
using BeaconColorUtils.Core.Models;
using BeaconColorUtils.Core.Processing;
using BeaconColorUtils.Core.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using fNbt;

namespace BeaconColorUtils.UI.ViewModels;

public partial class GradientGeneratorViewModel : ViewModelBase
{
    public HueInterpolationMode[] InterpolationModes { get; } = Enum.GetValues<HueInterpolationMode>();

    public ObservableCollection<ColorStopViewModel> ColorStops { get; } = [];

    private readonly BeaconColorService _colorService;

    private CancellationTokenSource? _calculationCts;

    [ObservableProperty] public partial decimal? Steps { get; set; } = 500;

    [ObservableProperty] public partial HueInterpolationMode SelectedInterpolationMode { get; set; } = HueInterpolationMode.Increasing;

    [ObservableProperty] public partial LinearGradientBrush FinalGradientBrush { get; set; } = new();

    [ObservableProperty] public partial int MaxLayers { get; set; } = 6;

    [ObservableProperty] public partial bool IncludeBeacons { get; set; } = true;

    [ObservableProperty] public partial bool UseGlassPanes { get; set; } = false;

    [ObservableProperty] public partial bool CompressDuplicates { get; set; } = true;

    [ObservableProperty] public partial double PreviewWidth { get; set; } = 1000;

    // ReSharper disable once UnusedParameterInPartialMethod
    partial void OnPreviewWidthChanged(double value) => UpdateGradientBrushAsync();

    // ReSharper disable once UnusedParameterInPartialMethod
    partial void OnSelectedInterpolationModeChanged(HueInterpolationMode value) => UpdateGradientBrushAsync();

    partial void OnStepsChanged(decimal? value)
    {
        if (value is null)
        {
            Dispatcher.UIThread.Post(() => { Steps = ColorStops.Count; });

            return;
        }

        UpdateGradientBrushAsync();
    }

    public GradientGeneratorViewModel(BeaconColorService colorService)
    {
        _colorService = colorService;

        var color1 = new ColorStopViewModel(generateRandomColor: true);
        color1.PropertyChanged += OnColorStopPropertyChanged;
        ColorStops.Add(color1);

        var color2 = new ColorStopViewModel(generateRandomColor: true);
        color2.PropertyChanged += OnColorStopPropertyChanged;
        ColorStops.Add(color2);

        ColorStops.CollectionChanged += (_, _) =>
        {
            OnPropertyChanged(nameof(ColorStops));
            UpdateCanRemoveState();
            UpdateGradientBrushAsync();
        };

        UpdateCanRemoveState();
        UpdateGradientBrushAsync();
    }

    private void UpdateCanRemoveState()
    {
        var canRemove = ColorStops.Count > 2;
        foreach (var stop in ColorStops)
        {
            stop.CanRemove = canRemove;
        }
    }

    private void OnColorStopPropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(ColorStopViewModel.Color))
            UpdateGradientBrushAsync();
    }

    [RelayCommand]
    private void AddColor()
    {
        var newColorStop = new ColorStopViewModel();
        newColorStop.PropertyChanged += OnColorStopPropertyChanged;

        ColorStops.Add(newColorStop);
    }

    [RelayCommand]
    private void RemoveColor(ColorStopViewModel item)
    {
        if (ColorStops.Count <= 2) return;

        item.PropertyChanged -= OnColorStopPropertyChanged;
        ColorStops.Remove(item);
    }

    [RelayCommand]
    private async Task ExportNbtAsync()
    {
        if (ColorStops.Count < 2)
            return;

        if (_calculationCts != null)
            await _calculationCts.CancelAsync();

        _calculationCts = new CancellationTokenSource();
        var token = _calculationCts.Token;
        var totalSteps = (int)(Steps ?? ColorStops.Count);

        var gradientColors = await CalculateGradientColorsAsync(totalSteps, token);

        if (token.IsCancellationRequested || gradientColors.Length == 0)
            return;

        Func<Task> action = MaxLayers switch
        {
            1 => () => ProcessAndSaveNbtAsync<byte>(gradientColors),
            <= 3 => () => ProcessAndSaveNbtAsync<ushort>(gradientColors),
            _ => () => ProcessAndSaveNbtAsync<int>(gradientColors)
        };

        await action();
        return;

        async Task ProcessAndSaveNbtAsync<T>(RgbColor[] colors) where T : struct, IBinaryInteger<T>
        {
            var nbt = await Task.Run(() =>
            {
                var finalSequences = new List<ColoredGlassSequence<T>>();

                foreach (var color in colors)
                {
                    var current = _colorService.GetBestMatch<T>(color, MaxLayers);

                    if (CompressDuplicates)
                    {
                        if (finalSequences.Count == 0 || !current.Value.Equals(finalSequences[^1].Value))
                        {
                            finalSequences.Add(current);
                        }
                    }
                    else
                    {
                        finalSequences.Add(current);
                    }
                }

                var glassSequenceArray = finalSequences.ToArray();

                return BeaconStructureBuilder.Build(glassSequenceArray, MaxLayers, IncludeBeacons, UseGlassPanes);
            }, token);

            if (token.IsCancellationRequested) return;

            if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                var storageProvider = desktop.MainWindow!.StorageProvider;

                var file = await storageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
                {
                    Title = "Save NBT Structure",
                    DefaultExtension = "nbt",
                    SuggestedFileName = "Beacon Gradient Structure",
                    FileTypeChoices =
                    [
                        new FilePickerFileType("Minecraft Structure NBT") { Patterns = ["*.nbt"] }
                    ]
                });

                if (file != null)
                {
                    var nbtFile = new NbtFile(nbt);

                    nbtFile.SaveToFile(file.Path.LocalPath, NbtCompression.GZip);
                }
            }
        }
    }

    /// <summary>
    /// Generates array of RgbColors
    /// </summary>
    private async Task<RgbColor[]> CalculateGradientColorsAsync(int targetSteps, CancellationToken token = default)
    {
        if (ColorStops.Count < 2 || targetSteps <= 0)
            return [];

        var segmentsCount = ColorStops.Count - 1;
        var mode = SelectedInterpolationMode;

        var colors = ColorStops.Select(c => c.Color).ToArray();

        return await Task.Run(() =>
        {
            var calculatedColors = new RgbColor[targetSteps];
            var currentIndex = 0;

            for (var i = 0; i < segmentsCount; i++)
            {
                token.ThrowIfCancellationRequested();

                var startColor = colors[i];
                var endColor = colors[i + 1];

                var start = new RgbColor(startColor.R, startColor.G, startColor.B);
                var end = new RgbColor(endColor.R, endColor.G, endColor.B);

                var segmentSteps = targetSteps * (i + 1) / segmentsCount - targetSteps * i / segmentsCount;

                var generatedColors = OklchGradientGenerator.GetGradient(start, end, segmentSteps, mode);

                foreach (var color in generatedColors.Where(_ => currentIndex < targetSteps))
                {
                    calculatedColors[currentIndex++] = color;
                }
            }

            return calculatedColors;
        }, token);
    }


    // ReSharper disable once AsyncVoidMethod
    private async void UpdateGradientBrushAsync()
    {
        if (ColorStops.Count < 2)
            return;

        if (_calculationCts != null)
            await _calculationCts.CancelAsync();

        _calculationCts = new CancellationTokenSource();
        var token = _calculationCts.Token;

        try
        {
            var totalSteps = (int)(Steps ?? ColorStops.Count);
            var previewSteps = Math.Min(totalSteps, Math.Max(10, (int)PreviewWidth));

            var allGeneratedColors = await CalculateGradientColorsAsync(previewSteps, token);

            if (token.IsCancellationRequested || allGeneratedColors.Length == 0)
                return;

            var brush = new LinearGradientBrush
            {
                StartPoint = new RelativePoint(0, 0, RelativeUnit.Relative),
                EndPoint = new RelativePoint(1, 0, RelativeUnit.Relative),
                GradientStops = []
            };

            var stepSize = 1.0 / allGeneratedColors.Length;

            for (var i = 0; i < allGeneratedColors.Length; i++)
            {
                var color = allGeneratedColors[i];
                var avaloniaColor = Color.FromRgb(color.R, color.G, color.B);

                brush.GradientStops.Add(new GradientStop(avaloniaColor, i * stepSize));
                brush.GradientStops.Add(new GradientStop(avaloniaColor, (i + 1) * stepSize));
            }

            FinalGradientBrush = brush;
        }
        catch (OperationCanceledException)
        {
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Unexpected Gradient generation error: {ex}");
        }
    }
}