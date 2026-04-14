using System.ComponentModel;
using BeaconColorUtils.Core.Attributes;

namespace BeaconColorUtils.Core.Enums;

public enum GlassColors : byte
{
    [GlassId("minecraft:white_stained_glass")]
    [GlassPaneId("minecraft:white_stained_glass_pane")]
    White = 0,

    [GlassId("minecraft:light_gray_stained_glass")]
    [GlassPaneId("minecraft:light_gray_stained_glass_pane")]
    LightGray = 1,

    [GlassId("minecraft:gray_stained_glass")]
    [GlassPaneId("minecraft:gray_stained_glass_pane")]
    Gray = 2,

    [GlassId("minecraft:black_stained_glass")]
    [GlassPaneId("minecraft:black_stained_glass_pane")]
    Black = 3,

    [GlassId("minecraft:brown_stained_glass")]
    [GlassPaneId("minecraft:brown_stained_glass_pane")]
    Brown = 4,

    [GlassId("minecraft:red_stained_glass")]
    [GlassPaneId("minecraft:red_stained_glass_pane")]
    Red = 5,

    [GlassId("minecraft:orange_stained_glass")]
    [GlassPaneId("minecraft:orange_stained_glass_pane")]
    Orange = 6,

    [GlassId("minecraft:yellow_stained_glass")]
    [GlassPaneId("minecraft:yellow_stained_glass_pane")]
    Yellow = 7,

    [GlassId("minecraft:lime_stained_glass")]
    [GlassPaneId("minecraft:lime_stained_glass_pane")]
    Lime = 8,

    [GlassId("minecraft:green_stained_glass")]
    [GlassPaneId("minecraft:green_stained_glass_pane")]
    Green = 9,

    [GlassId("minecraft:cyan_stained_glass")]
    [GlassPaneId("minecraft:cyan_stained_glass_pane")]
    Cyan = 10,

    [GlassId("minecraft:light_blue_stained_glass")]
    [GlassPaneId("minecraft:light_blue_stained_glass_pane")]
    LightBlue = 11,

    [GlassId("minecraft:blue_stained_glass")]
    [GlassPaneId("minecraft:blue_stained_glass_pane")]
    Blue = 12,

    [GlassId("minecraft:purple_stained_glass")]
    [GlassPaneId("minecraft:purple_stained_glass_pane")]
    Purple = 13,

    [GlassId("minecraft:magenta_stained_glass")]
    [GlassPaneId("minecraft:magenta_stained_glass_pane")]
    Magenta = 14,

    [GlassId("minecraft:pink_stained_glass")]
    [GlassPaneId("minecraft:pink_stained_glass_pane")]
    Pink = 15
}