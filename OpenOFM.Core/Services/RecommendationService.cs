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
            return _stations.GetAllRadioStations()
                .Select(station => (station, score: GetStationScore(_playlists.GetPlaylist(station.Id, DateTime.Now))))
                .OrderByDescending(x => x.score)
                .TakeWhile(x => x.score > 0)
                .Select(x => x.station).ToList();
        }

        private int GetStationScore(Playlist? playlist)
        {
            int score = 0;

            for (int i = 0; i < playlist?.Queue.Count; i++)
            {
                if (_favorites.CurrentSettings.FavoriteSongs.Contains(playlist.Queue[i]))
                {
                    score += playlist.Queue.Count - i;
                }
            }

            return score;
        }
    }
}
