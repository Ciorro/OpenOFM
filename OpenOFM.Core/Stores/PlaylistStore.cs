using OpenOFM.Core.Models;
using System.Collections.Concurrent;

namespace OpenOFM.Core.Stores
{
    public class PlaylistStore : IPlaylistStore
    {
        private readonly ConcurrentDictionary<int, Playlist> _playlists = new();

        public void AddPlaylist(Playlist playlist)
        {
            _playlists[playlist.RadioStationId] = playlist;
        }

        public Playlist? GetPlaylist(int stationId, DateTime dateFrom)
        {
            return _playlists.TryGetValue(stationId, out var playlist) ?
                playlist : null;
        }
    }
}
