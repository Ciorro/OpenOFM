using OpenOFM.Core.Models;

namespace OpenOFM.Core.Services.Playlists
{
    public delegate void PlaylistAvailableEventHandler(Playlist playlist);

    public interface IPlaylistService
    {
        event PlaylistAvailableEventHandler? PlaylistAvailable;
        void SetPlaylist(Playlist playlist);
        Playlist? GetPlaylist(int stationId, DateTime timeFrom);
    }
}
