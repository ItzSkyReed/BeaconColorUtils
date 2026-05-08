using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using ZstdSharp;
using System.Reflection;
using BeaconColorUtils.Core.Models;


namespace BeaconColorUtils.Infrastructure.Storage;

public class CacheSerializer
{
    // KD-Trees (2-5 layers)
    public Dictionary<int, OklabKdTree<short>.KdNode[]> KdTreesShort { get; } = new();
    public Dictionary<int, OklabKdTree<int>.KdNode[]> KdTreesInt { get; } = new();

    // LUTs (6-15 layers)
    public Dictionary<int, int[]> LutsInt { get; } = new();
    public Dictionary<int, long[]> LutsLong { get; } = new();

    private enum StructType : byte
    {
        KdTree = 0,
        Lut = 1
    }

    private enum DataType : byte
    {
        Short = 0,
        Int = 1,
        Long = 2
    }

    /// <summary>
    /// Loads all trees and LUTs from a single .zst file embedded in the assembly resources
    /// </summary>
    public static CacheSerializer LoadFromEmbeddedResource(string resourceName)
    {
        var storage = new CacheSerializer();

        var assembly = Assembly.GetExecutingAssembly();
        using var resourceStream = assembly.GetManifestResourceStream(resourceName);

        if (resourceStream == null)
            throw new FileNotFoundException($"Embedded resource '{resourceName}' not found.");

        using var zstd = new DecompressionStream(resourceStream);
        using var reader = new BinaryReader(zstd);

        var totalBlocks = reader.ReadInt32();

        for (var i = 0; i < totalBlocks; i++)
        {
            var layer = reader.ReadInt32();
            var sType = (StructType)reader.ReadByte();
            var dType = (DataType)reader.ReadByte();
            var byteLength = reader.ReadInt32();

            switch (sType)
            {
                case StructType.KdTree when dType == DataType.Short:
                    storage.KdTreesShort[layer] = ReadArray<OklabKdTree<short>.KdNode>(zstd, byteLength);
                    break;
                case StructType.KdTree when dType == DataType.Int:
                    storage.KdTreesInt[layer] = ReadArray<OklabKdTree<int>.KdNode>(zstd, byteLength);
                    break;
                case StructType.Lut when dType == DataType.Int:
                    storage.LutsInt[layer] = ReadArray<int>(zstd, byteLength);
                    break;
                case StructType.Lut when dType == DataType.Long:
                    storage.LutsLong[layer] = ReadArray<long>(zstd, byteLength);
                    break;
                default:
                    var discardBuffer = new byte[byteLength];
                    zstd.ReadExactly(discardBuffer);
                    break;
            }
        }

        return storage;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static T[] ReadArray<T>(DecompressionStream zstd, int byteLength) where T : unmanaged
    {
        var arrayLength = byteLength / Unsafe.SizeOf<T>();

        var array = GC.AllocateUninitializedArray<T>(arrayLength, pinned: true);
        var byteSpan = MemoryMarshal.AsBytes(array.AsSpan());

        zstd.ReadExactly(byteSpan);

        return array;
    }
}