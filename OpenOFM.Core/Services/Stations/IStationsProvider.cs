using OpenOFM.Core.Models;

namespace OpenOFM.Core.Services.Stations
{
    public interface IStationsProvider
    {
        Task<IReadOnlyCollection<RadioStation>> GetStations(CancellationToken ct);
    }
}
