using Microsoft.Extensions.Hosting;
using OpenOFM.Core.Api;
using OpenOFM.Core.Services.Playlists;
using System.Net.Http;

namespace OpenOFM.Ui.Services
{
    internal class PlaylistBackgroundService : BackgroundService
    {
        private readonly PlaylistApiClient _playlistApi;
        private readonly IPlaylistService _playlistService;

        public PlaylistBackgroundService(IPlaylistService playlistService, PlaylistApiClient playlistApi)
        {
            _playlistService = playlistService;
            _playlistApi = playlistApi;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var playlists = await _playlistApi.GetPlaylists(stoppingToken);

                    foreach (var playlist in playlists)
                    {
                        _playlistService.SetPlaylist(playlist);
                    }
                }
                catch (HttpRequestException) { }

                await Task.Delay(TimeSpan.FromSeconds(15));
            }
        }
    }
}
