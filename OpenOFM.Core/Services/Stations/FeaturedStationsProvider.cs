using OpenOFM.Core.Api;
using OpenOFM.Core.Models;

namespace OpenOFM.Core.Services.Stations
{
    public class FeaturedStationsProvider : IStationsProvider
    {
        private readonly IStationsProvider _stationsProvider;
        private readonly StationsApiClient _stationsApi;

        public FeaturedStationsProvider(IStationsProvider stationsProvider, StationsApiClient stationsApi)
        {
            _stationsProvider = stationsProvider;
            _stationsApi = stationsApi;
        }

        public async Task<IReadOnlyCollection<RadioStation>> GetStations(CancellationToken ct)
        {
            var getStationsTask = _stationsProvider.GetStations(ct);
            var getFeaturedStationsIdsTask = _stationsApi.GetFeaturedRadioStationsIds(ct);

            await Task.WhenAll(getStationsTask, getFeaturedStationsIdsTask);

            var stations = getStationsTask.Result;
            var featuredIds = getFeaturedStationsIdsTask.Result;

            return stations.Where(x => featuredIds.Contains(x.Id)).ToList();
        }
    }
}
