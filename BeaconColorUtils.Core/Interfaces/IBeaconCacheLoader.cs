using System.Threading.Tasks;
using BeaconColorUtils.Core.Cache;
using BeaconColorUtils.Core.Models;

namespace BeaconColorUtils.Core.Interfaces;

public interface IBeaconDataLoader
{
    Task<(SequenceLut<long> Lut8, SequenceLut<int> Lut7, SequenceLut<int> Lut6,
        OklabKdTree<int> KdTree5, OklabKdTree<int> KdTree4, OklabKdTree<short>KdTree3, OklabKdTree<short> KdTree2)> LoadCachesAsync();
}