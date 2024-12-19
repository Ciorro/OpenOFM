namespace OpenOFM.Core.Models
{
    public class Playlist
    {
        public int RadioStationId { get; }
        public List<Song> Queue { get; } = [];

        public Playlist(int radioStationId)
        {
            RadioStationId = radioStationId;
        }

        public Playlist(int radioStationId, IEnumerable<Song> queue)
            :this(radioStationId)
        {
            Queue = new List<Song>(queue);
        }
    }
}
