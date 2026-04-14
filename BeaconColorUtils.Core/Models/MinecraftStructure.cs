using BeaconColorUtils.Core.Enums;

namespace BeaconColorUtils.Core.Models;

public record MinecraftStructure
{
    public MinecraftStructure(int dataVersion, int[] size, GlassColors GlassColor) {
        if (size.Length != 3) {
            throw new ArgumentOutOfRangeException($"Size must be 3 or 4 but was {size.Length}");
        }
    }
}