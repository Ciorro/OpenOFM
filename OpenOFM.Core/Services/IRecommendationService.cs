using OpenOFM.Core.Models;

namespace OpenOFM.Core.Services
{
    public interface IRecommendationService
    {
        IEnumerable<RadioStation> GetRecommendedStations();
    }
}
