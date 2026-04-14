using System;
using BeaconColorUtils.Core.Enums;

namespace BeaconColorUtils.Core.Models;


public readonly record struct OklChColor(float L, float C, float H)
{
    /// <summary>
    /// Converts from Oklab to OklCh
    /// </summary>
    public static OklChColor FromOklab(OklabColor oklab)
    {
        var c = float.Hypot(oklab.A, oklab.B);
        var h = MathF.Atan2(oklab.B, oklab.A) * (180F / MathF.PI);

        return new OklChColor(oklab.L, c, h < 0 ? h + 360 : h);
    }

    public static OklChColor FromRgb(RgbColor rgb)
    {
        return FromOklab(OklabColor.FromRgb(rgb));
    }

    public OklChColor Interpolate(OklChColor target, float t, HueInterpolationMode mode = HueInterpolationMode.Increasing)
    {
        var l = L + (target.L - L) * t;
        var c = C + (target.C - C) * t;

        var h1 = H;
        var h2 = target.H;

        // Если цвет серый, его угол не имеет значения
        if (C < 0.0001f) h1 = h2;
        if (target.C < 0.0001f) h2 = h1;

        var deltaH = h2 - h1;

        switch (mode)
        {
            case HueInterpolationMode.Shorter:
                if (deltaH > 180) deltaH -= 360;
                else if (deltaH < -180) deltaH += 360;
                break;

            case HueInterpolationMode.Longer:
                if (deltaH > 0 && deltaH <= 180) deltaH -= 360;
                else if (deltaH > -180 && deltaH <= 0) deltaH += 360;
                break;

            case HueInterpolationMode.Increasing:
                if (deltaH < 0) deltaH += 360;
                break;

            case HueInterpolationMode.Decreasing:
                if (deltaH > 0) deltaH -= 360;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(mode), mode, null);
        }

        var interpolatedH = h1 + deltaH * t;

        switch (interpolatedH)
        {
            case < 0:
                interpolatedH += 360;
                break;
            case >= 360:
                interpolatedH -= 360;
                break;
        }

        return new OklChColor(l, c, interpolatedH);
    }
}