using OpenOFM.Core.Models;

namespace OpenOFM.Core.Stores
{
    public interface IStationsStore
    {
        void AddStation(RadioStation radioStation);
        IEnumerable<RadioStation> GetAllRadioStations();
        IEnumerable<RadioStation> GetRadioStationsByCategoryId(int categoryId);
    }
}
