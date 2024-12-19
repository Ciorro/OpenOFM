using OpenOFM.Core.Models;

namespace OpenOFM.Core.Services
{
    public interface IRadioStationsService
    {
        Task<IEnumerable<RadioStation>> GetAllRadioStations(CancellationToken ct = default);
        Task<IEnumerable<RadioStation>> GetFeaturedRadioStations(CancellationToken ct = default);
        Task<IEnumerable<RadioStation>> GetRecommendedRadioStations(CancellationToken ct = default);
        Task<IEnumerable<RadioStation>> GetRadioStationsByCategoryId(int categoryId, CancellationToken ct = default);
    }
}
