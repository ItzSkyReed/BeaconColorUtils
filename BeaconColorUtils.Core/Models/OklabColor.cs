namespace BeaconColorUtils.Core.Models;

public readonly record struct OklabColor(float L, float A, float B)
{
    private const float MaxOklabDistance = 1.5F;

    /// <summary>
    /// Converts from RGB to Oklab
    /// </summary>
    public static OklabColor FromRgb(RgbColor rgb)
    {
        var r = rgb.R / 255.0F;
        var g = rgb.G / 255.0F;
        var b = rgb.B / 255.0F;

        r = InverseGammaCorrection(r);
        g = InverseGammaCorrection(g);
        b = InverseGammaCorrection(b);

        var l = MathF.Cbrt(0.4122214708F * r + 0.5363325363F * g + 0.0514459929F * b);
        var m = MathF.Cbrt(0.2119034982F * r + 0.6806995451F * g + 0.1073969566F * b);
        var s = MathF.Cbrt(0.0883024619F * r + 0.2817188376F * g + 0.6299787005F * b);

        var okL = l * 0.2104542553F + m * 0.7936177850F + s * -0.0040720468F;
        var okA = l * 1.9779984951F + m * -2.4285922050F + s * 0.4505937099F;
        var okB = l * 0.0259040371F + m * 0.7827717662F + s * -0.8086757660F;

        return new OklabColor(okL, okA, okB);
    }

    /// <summary>
    /// Converts from OkLCh to Oklab
    /// </summary>
    public static OklabColor FromOklch(OklChColor oklch)
    {
        var hRad = oklch.H * (MathF.PI / 180.0F);
        var a = oklch.C * MathF.Cos(hRad);
        var b = oklch.C * MathF.Sin(hRad);
        return new OklabColor(oklch.L, a, b);
    }

    /// <summary>
    ///  sRGB -> Linear RGB
    /// </summary>
    private static float InverseGammaCorrection(float c)
    {
        return c <= 0.04045F
            ? c / 12.92F
            : MathF.Pow((c + 0.055F) / 1.055F, 2.4F);
    }

    public static float DeltaE(OklabColor firstColor, OklabColor secondColor)
    {
        var dL = secondColor.L - firstColor.L;
        var dA = secondColor.A - firstColor.A;
        var dB = secondColor.B - firstColor.B;

        return MathF.Sqrt(dL * dL + dA * dA + dB * dB);
    }

    public static float GetSimilarityScore(float deltaE)
    {
        var percentage = 100F * (1F - deltaE / MaxOklabDistance);

        return MathF.Max(0F, percentage);
    }
}