using OpenOFM.Core.Models;

namespace OpenOFM.Core.Settings
{
    public class Favorites
    {
        public HashSet<Song> FavoriteSongs { get; set; } = new();
    }
}
