using OpenOFM.Core.Models;

namespace OpenOFM.Core.Stores
{
    public interface IPlaylistStore
    {
        void AddPlaylist(Playlist playlist);
        Playlist? GetPlaylist(int stationId, DateTime dateFrom);
    }
}
