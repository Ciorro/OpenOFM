using OpenOFM.Core.Models;

namespace OpenOFM.Core.Services.Player
{
    public delegate void StationChangedEventHandler(object sender, RadioStation? newStation);

    public interface IPlayerService
    {
        event StationChangedEventHandler? StationChanged;

        float Volume { get; set; }
        bool IsPaused { get; set; }
        RadioStation? CurrentStation { get; }

        Task Play(RadioStation radioStation);
        Task PlayPrevious();
        Task PlayNext();
        void Stop();
        TimeSpan GetDelay();
    }
}
