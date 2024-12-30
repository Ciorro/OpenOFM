using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using OpenOFM.Core.Models;
using OpenOFM.Core.Services;
using OpenOFM.Core.Stores;
using OpenOFM.Ui.Messages;
using OpenOFM.Ui.Navigation.Attributes;

namespace OpenOFM.Ui.ViewModels.Pages
{
    [PageKey("Player")]
    internal partial class PlayerPageViewModel : BasePageViewModel
    {
        private readonly IPlayerService _playerService;
        private readonly IPlaylistStore _playlistStore;
        private readonly IStationsStore _stationsStore;

        [ObservableProperty]
        private RadioStation? _currentStation;

        [ObservableProperty]
        private Song? _currentSong;

        [ObservableProperty]
        private Song[] _upcomingSongs = [];

        public PlayerPageViewModel(IPlayerService playerService, IPlaylistStore playlistStore, IStationsStore stationsStore)
        {
            _playerService = playerService;
            _playlistStore = playlistStore;
            _stationsStore = stationsStore;

            WeakReferenceMessenger.Default.Register<PlaylistsUpdatedNotification>(this, (sender, _) =>
            {
                UpdatePlaylist();
            });
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
            CurrentSong = playlist?.Queue.ElementAtOrDefault(0);
            UpcomingSongs = (playlist?.Queue.Skip(1) ?? []).ToArray();
        }

        private void UpdateStation()
        {
            CurrentStation = _playerService?.CurrentStation;

            //if (CurrentStation is not null)
            //{
            //    RecommendedStations = CurrentStation.Categories
            //        .SelectMany(x => _stationsStore.GetRadioStationsByCategoryId(x.Id))
            //        .Select(x => new RadioStationItemViewModel(x))
            //        .ToArray();
            //}
        }
    }
}
