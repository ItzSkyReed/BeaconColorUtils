using System;
using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace BeaconColorUtils.UI.ViewModels;

public partial class ColorStopViewModel : ObservableObject
{
    [ObservableProperty]
    public partial Color Color { get; set; }

    [ObservableProperty]
    public partial string HexInputText { get; set; }

    [ObservableProperty]
    public partial bool CanRemove { get; set; }

    public ColorStopViewModel(bool generateRandomColor = true)
    {
        if (!generateRandomColor)
            Color = Colors.White;
        else
        {
            var i = Random.Shared.Next(3);
            var r = i == 0 ? (byte)255 : (byte)Random.Shared.Next(256);
            var g = i == 1 ? (byte)255 : (byte)Random.Shared.Next(256);
            var b = i == 2 ? (byte)255 : (byte)Random.Shared.Next(256);

            Color = Color.FromRgb(r, g, b);
        }

        HexInputText = $"{Color.R:X2}{Color.G:X2}{Color.B:X2}";
    }

    partial void OnColorChanged(Color value)
    {
        HexInputText = $"{value.R:X2}{value.G:X2}{value.B:X2}";
    }

    [RelayCommand]
    private void ValidateAndRevertHex()
    {
        var input = HexInputText.Trim();

        if (!input.StartsWith('#') && !string.IsNullOrEmpty(input))
        {
            input = "#" + input;
        }
        if (Color.TryParse(input, out var parsedColor))
        {
            Color = Color.FromRgb(parsedColor.R, parsedColor.G, parsedColor.B);
        }

        HexInputText = $"{Color.R:X2}{Color.G:X2}{Color.B:X2}";
    }
}