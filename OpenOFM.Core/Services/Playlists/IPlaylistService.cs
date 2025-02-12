using OpenOFM.Core.Models;

namespace OpenOFM.Core.Services.Playlists
{
    public interface IPlaylistService
    {
        Playlist GetPlaylist(int stationId, DateTime timeFrom);
    }
}
