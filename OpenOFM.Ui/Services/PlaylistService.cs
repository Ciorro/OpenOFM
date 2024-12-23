using Microsoft.Extensions.Hosting;
using OpenOFM.Core.Api;
using OpenOFM.Core.Stores;
using System.Net.Http;

namespace OpenOFM.Ui.Services
{
    internal class PlaylistService : BackgroundService
    {
        private readonly IPlaylistStore _playlists;
        private readonly PlaylistApiClient _playlistApi;

        public PlaylistService(IPlaylistStore playlists, PlaylistApiClient playlistApi)
        {
            _playlists = playlists;
            _playlistApi = playlistApi;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    foreach (var playlist in await _playlistApi.GetPlaylists(stoppingToken))
                    {
                        _playlists.AddPlaylist(playlist);
                    }
                }
                catch (HttpRequestException) { }

                await Task.Delay(TimeSpan.FromSeconds(15));
            }
        }
    }
}
