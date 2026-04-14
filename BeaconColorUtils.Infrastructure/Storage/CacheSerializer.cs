using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using ZstdSharp;

namespace BeaconColorUtils.Infrastructure.Storage;

public static class CacheSerializer
{
    private const int CacheSize = 16_777_216;

    public static T[] LoadFromEmbeddedResource<T>(string resourceName) where T : unmanaged, IBinaryInteger<T>
    {
        var assembly = typeof(CacheSerializer).Assembly;
        using var resourceStream = assembly.GetManifestResourceStream(resourceName);

        if (resourceStream == null)
            throw new FileNotFoundException($"Embedded resource '{resourceName}' not found.");

        var cacheData = GC.AllocateUninitializedArray<T>(CacheSize, pinned: true);
        using var zstdStream = new DecompressionStream(resourceStream);

        if (Unsafe.SizeOf<T>() == 4)
        {
            var cacheBytes = MemoryMarshal.AsBytes(cacheData.AsSpan());
            zstdStream.ReadExactly(cacheBytes);
        }

        else if (Unsafe.SizeOf<T>() == 2)
        {
            Span<uint> buffer = stackalloc uint[8192];
            var bufferBytes = MemoryMarshal.AsBytes(buffer);

            var totalWritten = 0;
            while (totalWritten < CacheSize)
            {
                var elementsToRead = Math.Min(buffer.Length, CacheSize - totalWritten);
                var bytesToRead = elementsToRead * sizeof(uint);

                zstdStream.ReadExactly(bufferBytes[..bytesToRead]);
                for (var i = 0; i < elementsToRead; i++)
                {
                    cacheData[totalWritten++] = T.CreateTruncating(buffer[i]);
                }
            }
        }
        else
        {
            throw new NotSupportedException($"Type size of {Unsafe.SizeOf<T>()} is not supported.");
        }

        return cacheData;
    }
}