using BeaconColorUtils.Core.Enums;
using BeaconColorUtils.Core.Models;

namespace BeaconColorUtils.Core.Processing;

public static class MinecraftBlender
{
    // Precalculated arrays of normalized colors (0.0..1.0)
    // Splitting into 3 arrays slightly speeds up access from the processor cache
    private static readonly float[] BaseR = new float[16];
    private static readonly float[] BaseG = new float[16];
    private static readonly float[] BaseB = new float[16];

    public static readonly uint[] HexColors =
    [
        0xf9fffe, 0x9d9d97, 0x474f52, 0x1d1d21, // White, LightGray, Gray, Black
        0x835432, 0xb02e26, 0xf9801d, 0xfed83d, // Brown, Red, Orange, Yellow
        0x80c71f, 0x5e7c16, 0x169c9c, 0x3ab3da, // Lime, Green, Cyan, LightBlue
        0x3c44aa, 0x8932b8, 0xc74ebd, 0xf38baa  // Blue, Purple, Magenta, Pink
    ];

    public static readonly string[] ColorNames =
    [
        "White", "LightGray", "Gray", "Black",
        "Brown", "Red", "Orange", "Yellow",
        "Lime", "Green", "Cyan", "LightBlue",
        "Blue", "Purple", "Magenta", "Pink"
    ];

    static MinecraftBlender()
    {

        for (var i = 0; i < 16; i++)
        {
            BaseR[i] = ((HexColors[i] >> 16) & 0xFF) / 255f;
            BaseG[i] = ((HexColors[i] >> 8) & 0xFF) / 255f;
            BaseB[i] = (HexColors[i] & 0xFF) / 255f;
        }
    }


    /// <summary>
    /// Mixes the glass sequence according to Minecraft rules.
    /// </summary>
    public static RgbColor Blend(ReadOnlySpan<GlassColors> sequence)
    {

        var firstColor = (int)sequence[0];
        var totalR = BaseR[firstColor];
        var totalG = BaseG[firstColor];
        var totalB = BaseB[firstColor];

        for (var i = 1; i < sequence.Length; i++)
        {
            var colorId = (int)sequence[i];
            totalR = (totalR + BaseR[colorId]) / 2f;
            totalG = (totalG + BaseG[colorId]) / 2f;
            totalB = (totalB + BaseB[colorId]) / 2f;
        }

        return new RgbColor(
            (byte)(totalR * 255f),
            (byte)(totalG * 255f),
            (byte)(totalB * 255f)
        );
    }
}