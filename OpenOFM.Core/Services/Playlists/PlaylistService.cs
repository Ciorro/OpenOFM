using OpenOFM.Core.Models;

namespace OpenOFM.Core.Services.Playlists
{
    public class PlaylistService : IPlaylistService
    {
        public event PlaylistAvailableEventHandler? PlaylistAvailable;
        private readonly Dictionary<int, Playlist> _playlists = [];

        public void SetPlaylist(Playlist playlist)
        {
            _playlists[playlist.RadioStationId] = playlist;
            PlaylistAvailable?.Invoke(playlist);
        }

        public Playlist? GetPlaylist(int stationId, DateTime timeFrom)
        {
            //TODO: Record playlist of a paused station.

            if (_playlists.TryGetValue(stationId, out var playlist))
            {
                return playlist;
            }

            return null;
        }
    }
}
