using BeaconColorUtils.Core.Interfaces;
using BeaconColorUtils.Core.Models;

namespace BeaconColorUtils.Infrastructure.Storage;

public class BeaconDataLoader : IBeaconDataLoader
{
    public async Task<(SequenceLut<long>, SequenceLut<int>, SequenceLut<int>, OklabKdTree<int>, OklabKdTree<int>, OklabKdTree<short>, OklabKdTree<short>)> LoadCachesAsync()
    {
        return await Task.Run(() =>
        {
            var cache = CacheSerializer.LoadFromEmbeddedResource("BeaconColorUtils.Infrastructure.Assets.beacon_caches.zst");
            var lut8 = new SequenceLut<long>(cache.LutsLong[8]);
            var lut7 = new SequenceLut<int>(cache.LutsInt[7]);
            var lut6 = new SequenceLut<int>(cache.LutsInt[6]);
            var kdTree5 = new OklabKdTree<int>(cache.KdTreesInt[5], cache.KdTreesInt[5].Length);
            var kdTree4 = new OklabKdTree<int>(cache.KdTreesInt[4], cache.KdTreesInt[4].Length);
            var kdTree3 = new OklabKdTree<short>(cache.KdTreesShort[3], cache.KdTreesShort[3].Length);
            var kdTree2 = new OklabKdTree<short>(cache.KdTreesShort[2], cache.KdTreesShort[2].Length);


            return (
                lut8, lut7, lut6, kdTree5, kdTree4, kdTree3, kdTree2
            );
        });
    }
}