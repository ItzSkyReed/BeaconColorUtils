using BeaconColorUtils.Core.Enums;

namespace BeaconColorUtils.Core.Processing;

using System.Collections.Generic;
using Models;

public static class OklchGradientGenerator
{
    public static List<RgbColor> GetGradient(RgbColor start, RgbColor end, int steps, HueInterpolationMode interpolationMode)
    {
        var result = new List<RgbColor>(steps);
        switch (steps)
        {
            case <= 0:
                return result;
            case 1:
                result.Add(start);
                return result;
        }

        var startOklch = OklChColor.FromRgb(start);
        var endOklch = OklChColor.FromRgb(end);

        for (var i = 0; i < steps; i++)
        {
            var t = (float)i / (steps - 1);

            var interpolatedOklch = startOklch.Interpolate(endOklch, t, interpolationMode);

            var oklab = OklabColor.FromOklch(interpolatedOklch);
            var rgbFloats = RgbColor.FromOklabToSrgb(oklab);
            var r = (byte)Math.Clamp(MathF.Round(rgbFloats.R), 0, 255);
            var g = (byte)Math.Clamp(MathF.Round(rgbFloats.G), 0, 255);
            var b = (byte)Math.Clamp(MathF.Round(rgbFloats.B), 0, 255);

            result.Add(new RgbColor(r, g, b));
        }

        return result;
    }
}