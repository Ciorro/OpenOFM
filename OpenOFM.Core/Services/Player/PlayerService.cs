using OpenOFM.Core.Api;
using OpenOFM.Core.Collections;
using OpenOFM.Core.Models;
using OpenOFM.Core.Settings;
using OpenOFM.Core.Settings.Configurations;

namespace OpenOFM.Core.Services.Player
{
    public class PlayerService : IPlayerService
    {
        public event StationChangedEventHandler? StationChanged;

        private readonly ISettingsProvider<AppSettings> _settings;
        private readonly TokenApiClient _tokenApi;
        private readonly HttpClient _httpClient;
        private readonly HistoryCollection<RadioStation> _history = new();

        private Streaming.Playback.Player? _player;
        private RadioStation? _currentStation;

        public PlayerService(ISettingsProvider<AppSettings> settings, TokenApiClient tokenApi)
        {
            _settings = settings;
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

            if (radioStation is not null)
            {
                _history.Push(radioStation);
            }
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

            _player = new Streaming.Playback.Player(new Uri(streamUrl), _httpClient, _settings.CurrentSettings.MaxDelay);
            _player.Play();
            _player.Volume = Volume;

            _currentStation = radioStation;
            StationChanged?.Invoke(this, radioStation);
        }
    }
}
