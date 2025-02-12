using OpenOFM.Core.Models;
using OpenOFM.Core.Services.Playlists;
using OpenOFM.Core.Settings;
using OpenOFM.Core.Settings.Configurations;

namespace OpenOFM.Core.Services.Stations
{
    public class RecommendedStationsProvider : IStationsProvider
    {
        private readonly HashSet<Song> _favoriteSongs;
        private readonly IStationsProvider _stationsProvider;
        private readonly IPlaylistService _playlistService;

        public RecommendedStationsProvider(IStationsProvider stationsProvider, IPlaylistService playlistService, ISettingsProvider<Favorites> favorites)
        {
            _stationsProvider = stationsProvider;
            _playlistService = playlistService;
            _favoriteSongs = favorites.CurrentSettings.FavoriteSongs;
        }

        public async Task<IReadOnlyCollection<RadioStation>> GetStations(CancellationToken ct)
        {
            var stations = await _stationsProvider.GetStations(ct);

            var recommendedStations = stations
                .Select(station => (station,
                    score: GetStationScore(_playlistService.GetPlaylist(station.Id, DateTime.Now))))
                .OrderByDescending(x => x.score)
                .TakeWhile(x => x.score > 0)
                .Select(x => x.station)
                .ToList();

            return recommendedStations;
        }

        private int GetStationScore(Playlist? playlist)
        {
            int score = 0;

            for (int i = 0; i < playlist?.Queue.Count; i++)
            {
                if (_favoriteSongs.Contains(playlist.Queue[i]))
                {
                    score += playlist.Queue.Count - i;
                }
            }

            return score;
        }
    }
}
