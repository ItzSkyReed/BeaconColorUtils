using System.Reflection;
using BeaconColorUtils.Core.Attributes;
using BeaconColorUtils.Core.Enums;

namespace BeaconColorUtils.Core.Cache;

public static class GlassIdsCache
{
    private static readonly Dictionary<GlassColors, string> BlockIds = new();
    private static readonly Dictionary<GlassColors, string> PaneIds = new();

    static GlassIdsCache()
    {
        foreach (var color in Enum.GetValues<GlassColors>())
        {
            BlockIds[color] = GetAttr<GlassIdAttribute>(color);
            PaneIds[color] = GetAttr<GlassPaneIdAttribute>(color);
        }
    }

    private static string GetAttr<T>(GlassColors color) where T : Attribute
    {
        var field = color.GetType().GetField(color.ToString());
        dynamic attr = field?.GetCustomAttribute(typeof(T))!;
        return attr.Id;
    }

    extension(GlassColors color)
    {
        public string GetGlassBlockId() => BlockIds[color];
        public string GetGlassPanelId() => PaneIds[color];
    }
}