using System.Threading.Tasks;
using BeaconColorUtils.Core.Cache;

namespace BeaconColorUtils.Core.Interfaces;

public interface IBeaconDataLoader
{
    Task<(
        SequenceCache<ushort> Cache3,
        SequenceCache<uint> Cache4,
        SequenceCache<uint> Cache5,
        SequenceCache<uint> Cache6
        )> LoadCachesAsync();
}