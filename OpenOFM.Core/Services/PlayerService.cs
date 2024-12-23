using OpenOFM.Core.Api;
using OpenOFM.Core.Models;
using OpenOFM.Core.Streaming.Playback;

namespace OpenOFM.Core.Services
{
    public class PlayerService : IPlayerService
    {
        private readonly TokenApiClient _tokenApi;
        private readonly HttpClient _httpClient;

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

        public TimeSpan Delay
        {
            get => _player?.Delay ?? TimeSpan.Zero;
        }

        public RadioStation? CurrentStation
        {
            get => _currentStation;
        }

        public async Task Play(RadioStation radioStation)
        {
            if (_player is not null)
            {
                Stop();
            }

            var streamUrl = await _tokenApi.AppendToken(radioStation.StreamUrl!);

            _player = new Player(new Uri(streamUrl), _httpClient);
            _player.Play();
            _player.Volume = Volume;

            _currentStation = radioStation;
        }

        public void Stop()
        {
            _player?.Stop();
            _player?.Dispose();
            _player = null;

            _currentStation = null;
        }
    }
}
