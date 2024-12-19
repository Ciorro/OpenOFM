using OpenOFM.Core.Models;

namespace OpenOFM.Core.Api.DTO
{
    internal class PlaylistDTO
    {
        public Song? CurrentSong { get; set; }
        public List<Song> Playlist { get; set; } = [];
    }
}
