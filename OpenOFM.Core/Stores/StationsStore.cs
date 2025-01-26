using OpenOFM.Core.Models;
using System.Collections.Concurrent;

namespace OpenOFM.Core.Stores
{
    public class StationsStore : IStationsStore
    {
        private readonly ConcurrentDictionary<int, RadioStation> _stations = new();

        public void AddStation(RadioStation radioStation)
        {
            _stations[radioStation.Id] = radioStation;
        }

        public IEnumerable<RadioStation> GetAllRadioStations()
        {
            return _stations.Values
                .Where(x => !x.IsPremium)
                .OrderBy(x => x.Id);
        }
    }
}
