using System.ComponentModel;

namespace BeaconColorUtils.Core.Enums;

public enum HueInterpolationMode
{
    [Description("Shorter Path")]
    Shorter,
    [Description("Longer Path")]
    Longer,
    [Description("Increasing (CW)")]
    Increasing,
    [Description("Decreasing (CCW)")]
    Decreasing
}