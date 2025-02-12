using OpenOFM.Core.Models;
using OpenOFM.Core.Stores;

namespace OpenOFM.Core.Services.Playlists
{
    public class PlaylistService : IPlaylistService
    {
        private readonly IStore<IReadOnlyCollection<Playlist>> _playlistStore;

        public PlaylistService(IStore<IReadOnlyCollection<Playlist>> playlistStore)
        {
            _playlistStore = playlistStore;
        }

        public Playlist GetPlaylist(int stationId, DateTime timeFrom)
        {
            return _playlistStore.Value?.Single(x => x.RadioStationId == stationId) ?? new(stationId);
        }
    }
}
