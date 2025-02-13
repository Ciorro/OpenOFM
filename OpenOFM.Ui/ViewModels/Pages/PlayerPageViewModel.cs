using CommunityToolkit.Mvvm.ComponentModel;
using OpenOFM.Core.Models;
using OpenOFM.Core.Services.Player;
using OpenOFM.Core.Services.Playlists;
using OpenOFM.Core.Settings;
using OpenOFM.Core.Settings.Configurations;
using OpenOFM.Ui.Navigation.Attributes;
using OpenOFM.Ui.ViewModels.Items;

namespace OpenOFM.Ui.ViewModels.Pages
{
    [PageKey("Player")]
    internal partial class PlayerPageViewModel : BasePageViewModel
    {
        private readonly IPlaylistService _playlistService;
        private readonly IPlayerService _playerService;
        private readonly ISettingsProvider<Favorites> _favorites;

        private SongItemViewModel[] _playlist = [];

        [ObservableProperty]
        private RadioStation? _currentStation;

        public PlayerPageViewModel(IPlaylistService playlistService, IPlayerService playerService, ISettingsProvider<Favorites> favorites)
        {
            _favorites = favorites;

            _playlistService = playlistService;
            _playlistService.PlaylistAvailable += (playlist) =>
            {
                if (playlist.RadioStationId == CurrentStation?.Id)
                {
                    UpdatePlaylist();
                }
            };

            _playerService = playerService;
            _playerService.StationChanged += (sender, station) =>
            {
                if (station is not null)
                {
                    UpdateStation();
                    UpdatePlaylist();
                }
            };
        }

        public SongItemViewModel? CurrentSong
        {
            get => _playlist.ElementAtOrDefault(0);
        }

        public IEnumerable<SongItemViewModel> UpcomingSongs
        {
            get => _playlist.Skip(1);
        }

        public override void OnResumed()
        {
            UpdateStation();
            UpdatePlaylist();
        }

        private void UpdateStation()
        {
            CurrentStation = _playerService.CurrentStation;
        }

        private void UpdatePlaylist()
        {
            var id = _playerService.CurrentStation!.Id;
            var timeFrom = DateTime.Now - _playerService.GetDelay();
            var playlist = _playlistService.GetPlaylist(id, timeFrom);

            if (playlist is not null && playlist.Queue.Any())
            {
                _playlist = playlist.Queue
                    .Select(x => new SongItemViewModel(x)
                    {
                        IsFavorite = _favorites.CurrentSettings.FavoriteSongs.Contains(x),
                        IsFavoriteChanged = OnIsFavoriteChanged
                    })
                    .ToArray();

                OnPropertyChanged(nameof(CurrentSong));
                OnPropertyChanged(nameof(UpcomingSongs));
            }
        }

        private void OnIsFavoriteChanged(SongItemViewModel sender, bool isFavorite)
        {
            if (isFavorite)
            {
                _favorites.CurrentSettings.FavoriteSongs.Add(sender.Song);
            }
            else
            {
                _favorites.CurrentSettings.FavoriteSongs.Remove(sender.Song);
            }

            _favorites.Save();
        }
    }
}
