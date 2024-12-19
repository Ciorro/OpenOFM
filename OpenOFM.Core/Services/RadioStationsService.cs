using OpenOFM.Core.Api;
using OpenOFM.Core.Models;

namespace OpenOFM.Core.Services
{
    public class RadioStationsService : IRadioStationsService
    {
        private readonly StationsApiClient _stationsApi;
        private readonly List<RadioStation> _stations = [];
        private readonly SemaphoreSlim _semaphore = new(1);

        public RadioStationsService(StationsApiClient stationsApi)
        {
            _stationsApi = stationsApi;
        }

        public async Task<IEnumerable<RadioStation>> GetAllRadioStations(CancellationToken ct)
        {
            try
            {
                await _semaphore.WaitAsync(ct);

                if (_stations.Count == 0)
                {
                    await LoadStations(ct);
                }

                return _stations;
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public async Task<IEnumerable<RadioStation>> GetFeaturedRadioStations(CancellationToken ct)
        {
            var stations = await GetAllRadioStations(ct);
            var featuredIds = await _stationsApi.GetFeaturedRadioStationsIds(ct);

            return stations.Where(x => featuredIds.Contains(x.Id));
        }

        public Task<IEnumerable<RadioStation>> GetRecommendedRadioStations(CancellationToken ct)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<RadioStation>> GetRadioStationsByCategoryId(int categoryId, CancellationToken ct)
        {
            return (await GetAllRadioStations(ct))
                .Where(x => x.Categories.Any(x => x.Id == categoryId));
        }

        private async Task LoadStations(CancellationToken ct)
        {
            //TODO: Cache stations locally.
            _stations.Clear();
            _stations.AddRange(await _stationsApi.GetRadioStations(ct));
        }
    }
}
