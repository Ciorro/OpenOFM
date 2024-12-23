using OpenOFM.Core.Models;

namespace OpenOFM.Core.Services
{
    public interface IPlayerService
    {
        float Volume { get; set; }
        bool IsPaused { get; set; }
        TimeSpan Delay { get; }
        RadioStation? CurrentStation { get; }

        Task Play(RadioStation radioStation);
        void Stop();
    }
}
