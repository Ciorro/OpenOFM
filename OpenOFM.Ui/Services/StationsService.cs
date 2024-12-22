using Microsoft.Extensions.Hosting;
using OpenOFM.Core.Api;
using OpenOFM.Core.Stores;

namespace OpenOFM.Ui.Services
{
    internal class StationsService : BackgroundService
    {
        private readonly IStationsStore _stations;
        private readonly StationsApiClient _stationsApi;

        public StationsService(IStationsStore stations, StationsApiClient stationsApi)
        {
            _stations = stations;
            _stationsApi = stationsApi;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {


                foreach (var station in await _stationsApi.GetRadioStations(stoppingToken))
                {
                    _stations.AddStation(station);
                }

                await Task.Delay(TimeSpan.FromDays(1));
            }
        }
    }
}
