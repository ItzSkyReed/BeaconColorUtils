using System;
using System.Collections.Generic;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using BeaconColorUtils.Core.Enums;

namespace BeaconColorUtils.UI.Models;

public record GlassPane(GlassColors Color, string DisplayName, Bitmap Image)
{
    private const string BasePath = "avares://BeaconColorUtils/Assets/Minecraft/GlassPanes/";

    private static readonly Dictionary<GlassColors, GlassPane> All;

    static GlassPane()
    {
        var allPanes = new Dictionary<GlassColors, GlassPane>();

        foreach (var color in Enum.GetValues<GlassColors>())
        {
            var fileName = $"{color}.png";

            var displayName = color.ToString();

            allPanes[color] = new GlassPane(color, displayName, LoadIcon(fileName));
        }

        All = allPanes;
    }

    private static Bitmap LoadIcon(string fileName)
    {
        var uri = new Uri($"{BasePath}{fileName}");
        return new Bitmap(AssetLoader.Open(uri));
    }

    public static GlassPane FromColor(GlassColors color) => All[color];
}