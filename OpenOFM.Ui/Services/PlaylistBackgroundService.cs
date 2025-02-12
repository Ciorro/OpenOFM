using Microsoft.Extensions.Hosting;
using OpenOFM.Core.Api;
using OpenOFM.Core.Models;
using OpenOFM.Core.Stores;
using System.Net.Http;

namespace OpenOFM.Ui.Services
{
    internal class PlaylistBackgroundService : BackgroundService
    {
        private readonly PlaylistApiClient _playlistApi;
        private readonly IStore<IReadOnlyCollection<Playlist>> _playlists;

        public PlaylistBackgroundService(IStore<IReadOnlyCollection<Playlist>> playlists, PlaylistApiClient playlistApi)
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
                    _playlists.SetValue((await _playlistApi.GetPlaylists(stoppingToken)).ToHashSet());
                }
                catch (HttpRequestException) { }

                await Task.Delay(TimeSpan.FromSeconds(15));
            }
        }
    }
}
