using OpenOFM.Core.Models;
using OpenOFM.Core.Settings;
using OpenOFM.Core.Settings.Configurations;
using OpenOFM.Core.Stores;

namespace OpenOFM.Core.Services
{
    public class RecommendationService : IRecommendationService
    {
        private readonly IStationsStore _stations;
        private readonly IPlaylistStore _playlists;
        private readonly ISettingsProvider<Favorites> _favorites;

        public RecommendationService(IStationsStore stations, IPlaylistStore playlists, ISettingsProvider<Favorites> favorites)
        {
            _stations = stations;
            _playlists = playlists;
            _favorites = favorites;
        }

        public IEnumerable<RadioStation> GetRecommendedStations()
        {
            return _stations.GetAllRadioStations().Where(station =>
            {
                var playlist = _playlists.GetPlaylist(station.Id, DateTime.Now);
                return _favorites.CurrentSettings.FavoriteSongs.Overlaps(playlist?.Queue ?? []);
            }).ToList();
        }
    }
}
