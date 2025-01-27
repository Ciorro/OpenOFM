using OpenOFM.Core.Api;
using OpenOFM.Core.Collections;
using OpenOFM.Core.Models;
using OpenOFM.Core.Streaming.Playback;

namespace OpenOFM.Core.Services
{
    public class PlayerService : IPlayerService
    {
        public event StationChangedEventHandler? StationChanged;

        private readonly TokenApiClient _tokenApi;
        private readonly HttpClient _httpClient;
        private readonly HistoryCollection<RadioStation> _history = new();

        private Player? _player;
        private RadioStation? _currentStation;

        public PlayerService(TokenApiClient tokenApi)
        {
            _tokenApi = tokenApi;

            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Add(
                "User-Agent", "BULDOZER449");
        }

        private float _volume = 1;
        public float Volume
        {
            get => _volume;
            set
            {
                _volume = value;
                if (_player is not null)
                {
                    _player.Volume = value;
                }
            }
        }

        public bool IsPaused
        {
            get => _player?.IsPaused ?? false;
            set
            {
                if (_player is not null)
                {
                    _player.IsPaused = value;
                }
            }
        }

        public RadioStation? CurrentStation
        {
            get => _currentStation;
        }

        public async Task Play(RadioStation radioStation)
        {
            await PlayInternal(radioStation);
            _history.Push(radioStation);
        }

        public async Task PlayPrevious()
        {
            if (_history.TryBack(out var station))
            {
                await PlayInternal(station);
            }
        }

        public async Task PlayNext()
        {
            if (_history.TryForward(out var station))
            {
                await PlayInternal(station);
            }
        }

        public void Stop()
        {
            _player?.Stop();
            _player?.Dispose();
            _player = null;

            _currentStation = null;
            StationChanged?.Invoke(this, null);
        }

        public TimeSpan GetDelay()
        {
            return _player?.Delay ?? TimeSpan.Zero;
        }

        private async Task PlayInternal(RadioStation radioStation)
        {
            var streamUrl = await _tokenApi.AppendToken(radioStation.StreamUrl!);

            if (_player is not null)
            {
                Stop();
            }

            _player = new Player(new Uri(streamUrl), _httpClient);
            _player.Play();
            _player.Volume = Volume;

            _currentStation = radioStation;
            StationChanged?.Invoke(this, radioStation);
        }
    }
}
