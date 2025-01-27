using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using OpenOFM.Core.Models;
using OpenOFM.Core.Services;
using OpenOFM.Core.Settings;
using OpenOFM.Core.Settings.Configurations;
using OpenOFM.Core.Stores;
using OpenOFM.Ui.Messages;
using OpenOFM.Ui.Navigation.Attributes;
using OpenOFM.Ui.ViewModels.Items;

namespace OpenOFM.Ui.ViewModels.Pages
{
    [PageKey("Player")]
    internal partial class PlayerPageViewModel : BasePageViewModel
    {
        private readonly IPlayerService _playerService;
        private readonly IPlaylistStore _playlistStore;
        private readonly ISettingsProvider<Favorites> _favorites;

        private SongItemViewModel[] _playlist = [];

        [ObservableProperty]
        private RadioStation? _currentStation;

        public PlayerPageViewModel(
            IPlayerService playerService,
            IPlaylistStore playlistStore,
            ISettingsProvider<Favorites> favorites)
        {
            _favorites = favorites;
            _playlistStore = playlistStore;
            _playerService = playerService;
            _playerService.StationChanged += (sender, station) =>
            {
                if (station is not null)
                {
                    UpdateStation();
                    UpdatePlaylist();
                }
            };

            WeakReferenceMessenger.Default.Register<PlaylistsUpdatedNotification>(this, (sender, _) =>
            {
                UpdatePlaylist();
            });
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

        private void UpdatePlaylist()
        {
            var id = _playerService.CurrentStation!.Id;
            var timeFrom = DateTime.Now - _playerService.GetDelay();
            var playlist = _playlistStore.GetPlaylist(id, timeFrom);

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

        private void UpdateStation()
        {
            CurrentStation = _playerService?.CurrentStation;
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
