using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.Hosting;
using OpenOFM.Core.Api;
using OpenOFM.Core.Stores;
using OpenOFM.Ui.Messages;
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
                    var playlists = await _playlistApi.GetPlaylists(stoppingToken);
                    foreach (var playlist in playlists)
                    {
                        _playlists.AddPlaylist(playlist);
                    }

                    WeakReferenceMessenger.Default.Send(new PlaylistsUpdatedNotification());
                }
                catch (HttpRequestException) { }

                await Task.Delay(TimeSpan.FromSeconds(15));
            }
        }
    }
}
