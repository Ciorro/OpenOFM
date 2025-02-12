using OpenOFM.Core.Api;
using OpenOFM.Core.Models;

namespace OpenOFM.Core.Services.Stations
{
    public class ApiStationsProvider : IStationsProvider, IDisposable
    {
        private readonly StationsApiClient _stationsApi;
        private readonly List<RadioStation> _stations = [];
        private readonly SemaphoreSlim _semaphore = new(1);

        public ApiStationsProvider(StationsApiClient stationsApi)
        {
            _stationsApi = stationsApi;
        }

        public async Task<IReadOnlyCollection<RadioStation>> GetStations(CancellationToken ct)
        {
            await _semaphore.WaitAsync(ct);

            try
            {
                if (_stations.Count == 0)
                {
                    var stations = (await _stationsApi.GetRadioStations(ct))
                        .Where(x => !x.IsPremium).ToList();
                    _stations.AddRange(stations);
                }

                return _stations;
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public void Dispose()
        {
            _semaphore.Dispose();
        }
    }
}
