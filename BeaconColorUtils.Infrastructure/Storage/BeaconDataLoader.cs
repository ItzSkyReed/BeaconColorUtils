using BeaconColorUtils.Core.Cache;
using BeaconColorUtils.Core.Interfaces;

namespace BeaconColorUtils.Infrastructure.Storage;

public class BeaconDataLoader : IBeaconDataLoader
{
    public async Task<(SequenceCache<ushort>, SequenceCache<uint>, SequenceCache<uint>, SequenceCache<uint>)> LoadCachesAsync()
    {
        return await Task.Run(() =>
        {
            var data3 = CacheSerializer.LoadFromEmbeddedResource<ushort>("BeaconColorUtils.Infrastructure.Assets.cache_3_glasses.zst");
            var data4 = CacheSerializer.LoadFromEmbeddedResource<uint>("BeaconColorUtils.Infrastructure.Assets.cache_4_glasses.zst");
            var data5 = CacheSerializer.LoadFromEmbeddedResource<uint>("BeaconColorUtils.Infrastructure.Assets.cache_5_glasses.zst");
            var data6 = CacheSerializer.LoadFromEmbeddedResource<uint>("BeaconColorUtils.Infrastructure.Assets.cache_6_glasses.zst");

            return (
                new SequenceCache<ushort>(data3),
                new SequenceCache<uint>(data4),
                new SequenceCache<uint>(data5),
                new SequenceCache<uint>(data6)
            );
        });
    }
}