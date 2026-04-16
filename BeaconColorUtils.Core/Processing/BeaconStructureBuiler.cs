using System.Numerics;
using BeaconColorUtils.Core.Cache;
using BeaconColorUtils.Core.Enums;
using BeaconColorUtils.Core.Models;
using fNbt;

namespace BeaconColorUtils.Core.Processing;

public static class BeaconStructureBuilder
{
    public static byte[] Build<T>(ColoredGlassSequence<T>[] sequence, int maxLayers, bool includeBeacons = true, bool useGlassPanes = false) where T : struct, IBinaryInteger<T>
    {
        var paletteList = new List<string>();
        var paletteMap = new Dictionary<string, int>();
        var colorStateCache = new int[16];

        for (var i = 0; i < 16; i++)
        {
            var color = (GlassColors)i;

            var blockStringId = useGlassPanes
                ? color.GetGlassPanelId()
                : color.GetGlassBlockId();

            colorStateCache[i] = GetOrAddId(blockStringId);
        }

        GetOrAddId("minecraft:air");

        var totalBlocks = 0;
        // ReSharper disable once LoopCanBeConvertedToQuery
        foreach (var glassSequence in sequence)
        {
            totalBlocks += Math.Min(glassSequence.Count, maxLayers) + (includeBeacons ? 1 : 0);
        }

        using var ms = new MemoryStream();
        var writer = new NbtWriter(ms, "root");

        writer.WriteInt("DataVersion", 3105);
        writer.WriteString("author", "https://github.com/ItzSkyReed");
        writer.WriteIntArray("size", [1, Convert.ToInt32(includeBeacons) + maxLayers, sequence.Length]);

        writer.BeginList("blocks", NbtTagType.Compound, totalBlocks);

        for (var z = 0; z < sequence.Length; z++)
        {
            var glassSequence = sequence[z];
            var yOffset = 0;

            if (includeBeacons)
            {
                var stateId = GetOrAddId("minecraft:beacon");

                writer.BeginCompound();
                writer.WriteIntArray("pos", [0, yOffset, z]);
                writer.WriteInt("state", stateId);
                writer.EndCompound();

                yOffset++;
            }

            var layers = Math.Min(glassSequence.Count, maxLayers);
            for (var y = 0; y < layers; y++)
            {
                var color = sequence[z][y];
                var stateId = colorStateCache[(int)color];

                writer.BeginCompound();
                writer.WriteIntArray("pos", [0, yOffset + y, z]);
                writer.WriteInt("state", stateId);
                writer.EndCompound();
            }
        }

        writer.EndList();

        writer.BeginList("palette", NbtTagType.Compound, paletteList.Count);
        foreach (var blockName in paletteList)
        {
            writer.BeginCompound();
            writer.WriteString("Name", blockName);
            writer.EndCompound();
        }

        writer.EndList();

        writer.EndCompound();
        writer.Finish();

        return ms.ToArray();

        int GetOrAddId(string blockName)
        {
            if (paletteMap.TryGetValue(blockName, out var id))
                return id;

            id = paletteList.Count;
            paletteMap[blockName] = id;
            paletteList.Add(blockName);
            return id;
        }
    }
}