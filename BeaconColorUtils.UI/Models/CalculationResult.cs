using System;
using System.Collections.Generic;
using System.Linq;

namespace BeaconColorUtils.UI.Models;

public record CalculationResult(float Accuracy, float DeltaE, string ResultColorHex, List<GlassPane> GlassPanes)
{
    public virtual bool Equals(CalculationResult? other)
    {
        return other is not null && GlassPanes.SequenceEqual(other.GlassPanes);
    }

    public override int GetHashCode()
    {
        var hash = new HashCode();

        foreach (var pane in GlassPanes)
        {
            hash.Add(pane);
        }

        return hash.ToHashCode();
    }
}