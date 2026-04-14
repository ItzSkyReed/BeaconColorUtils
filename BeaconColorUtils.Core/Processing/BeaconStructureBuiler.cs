using System.Numerics;
using BeaconColorUtils.Core.Cache;
using BeaconColorUtils.Core.Models;
using fNbt;

namespace BeaconColorUtils.Core.Processing;

public static class BeaconStructureBuilder
{
    public static NbtCompound Build<T>(ColoredGlassSequence<T>[] sequence, int maxLayers, bool includeBeacons = true, bool useGlassPanes = false) where T: struct, IBinaryInteger<T>
    {
        var root = CreateRootCompound(sequence.Length, maxLayers, includeBeacons);

        var paletteManager = new PaletteManager();
        var blocksList = new NbtList("blocks");

        paletteManager.GetOrAddId("minecraft:air");

        for (var z = 0; z < sequence.Length; z++)
        {
            BuildBeaconColumn(sequence[z], z, maxLayers, includeBeacons, useGlassPanes, blocksList, paletteManager);
        }

        root.Add(paletteManager.GetPaletteList());
        root.Add(blocksList);

        return root;
    }

    private static NbtCompound CreateRootCompound(int zSize, int maxLayers, bool includeBeacons)
    {
        const int xSize = 1;
        var ySize = Convert.ToInt32(includeBeacons) + maxLayers;

        return new NbtCompound("")
        {
            new NbtInt("DataVersion", 3105),
            new NbtString("author", "https://github.com/ItzSkyReed"),
            new NbtList("size", [
                new NbtInt(xSize),
                new NbtInt(ySize),
                new NbtInt(zSize)
            ])
        };
    }

    private static void BuildBeaconColumn<T>(
        ColoredGlassSequence<T> sequence,
        int zCoord,
        int maxLayers,
        bool includeBeacons,
        bool useGlassPanes,
        NbtList blocksList,
        PaletteManager paletteManager) where T: struct, IBinaryInteger<T>
    {
        var glassSequence = sequence.ToArray();
        var yOffset = 0;

        if (includeBeacons)
        {
            AddBlock(blocksList, paletteManager, "minecraft:beacon", 0, yOffset, zCoord);
            yOffset++;
        }

        for (var y = 0; y < glassSequence.Length && y < maxLayers; y++)
        {
            var blockId = useGlassPanes
                ? glassSequence[y].GetGlassPanelId()
                : glassSequence[y].GetGlassBlockId();

            AddBlock(blocksList, paletteManager, blockId, 0, yOffset + y, zCoord);
        }
    }

    private static void AddBlock(NbtList blocksList, PaletteManager paletteManager, string blockId, int x, int y, int z)
    {
        var stateId = paletteManager.GetOrAddId(blockId);

        blocksList.Add(new NbtCompound
        {
            new NbtList("pos", [new NbtInt(x), new NbtInt(y), new NbtInt(z)]),
            new NbtInt("state", stateId)
        });
    }

    private class PaletteManager
    {
        private readonly NbtList _paletteList = new("palette");
        private readonly Dictionary<string, int> _paletteMap = new();

        public int GetOrAddId(string blockName)
        {
            if (_paletteMap.TryGetValue(blockName, out var id))
                return id;

            id = _paletteMap.Count;
            _paletteMap[blockName] = id;

            _paletteList.Add(new NbtCompound
            {
                new NbtString("Name", blockName)
            });

            return id;
        }

        public NbtList GetPaletteList() => _paletteList;
    }
}