using OpenOFM.Core.Models;

namespace OpenOFM.Core.Services
{
    public interface IFeaturedService
    {
        Task<IEnumerable<RadioStation>> GetFeaturedStations(CancellationToken ct);
    }
}
