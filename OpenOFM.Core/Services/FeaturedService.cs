using OpenOFM.Core.Api;
using OpenOFM.Core.Models;
using OpenOFM.Core.Stores;

namespace OpenOFM.Core.Services
{
    public class FeaturedService : IFeaturedService
    {
        private readonly IStationsStore _stations;
        private readonly StationsApiClient _stationsApi;

        public FeaturedService(IStationsStore stations, StationsApiClient stationsApi)
        {
            _stations = stations;
            _stationsApi = stationsApi;
        }

        public async Task<IEnumerable<RadioStation>> GetFeaturedStations(CancellationToken ct)
        {
            var featuredStationsIds = await _stationsApi.GetFeaturedRadioStationsIds(ct);

            return _stations.GetAllRadioStations()
                .Where(x => featuredStationsIds.Contains(x.Id))
                .ToList();
        }
    }
}
