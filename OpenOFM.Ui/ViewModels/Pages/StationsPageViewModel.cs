using CommunityToolkit.Mvvm.ComponentModel;
using OpenOFM.Core.Models;
using OpenOFM.Core.Services;
using OpenOFM.Core.Stores;
using OpenOFM.Ui.Messages;
using OpenOFM.Ui.Navigation.Attributes;
using OpenOFM.Ui.ViewModels.Items;
using System.Windows.Threading;

namespace OpenOFM.Ui.ViewModels.Pages
{
    [PageKey("RadioStations")]
    internal partial class StationsPageViewModel : BasePageViewModel
    {
        private readonly IStationsStore _stations;
        private readonly IPlaylistStore _playlists;
        private readonly IPlayerService _player;
        private readonly DispatcherTimer _refreshTimer;

        [ObservableProperty]
        private List<RadioStationItemViewModel> _radioStations = new();

        public StationsPageViewModel(IStationsStore stations, IPlaylistStore playlists, IPlayerService player)
        {
            _stations = stations;
            _playlists = playlists;
            _player = player;

            _refreshTimer = new DispatcherTimer();
            _refreshTimer.Interval = TimeSpan.FromSeconds(15);
            _refreshTimer.Tick += (_, __) =>
            {
                RefreshPlaylists();
            };
            _refreshTimer.Start();
        }

        public override void OnResumed()
        {
            PopulateStations();
            RefreshPlaylists();

            if (_player.CurrentStation is not null)
            {
                MarkStation(_player.CurrentStation);
            }
        }

        public void Receive(RadioStationChangedMessage message)
        {
            foreach (var station in RadioStations!)
            {
                station.IsPlaying = station.Id == message.Value.Id;
            }
        }

        private void PopulateStations()
        {
            RadioStations = new(_stations.GetAllRadioStations()
                .Select(x => new RadioStationItemViewModel(x)
                {
                    OnSelected = OnStationSelected
                }));
        }

        private void RefreshPlaylists()
        {
            foreach (var station in RadioStations!)
            {
                station.Playlist = _playlists.GetPlaylist(station.Id, DateTime.Now);
            }
        }

        private void MarkStation(RadioStation radioStation)
        {
            foreach (var station in RadioStations!)
            {
                station.IsPlaying = station.Id == radioStation.Id;
            }
        }

        private void OnStationSelected(RadioStation radioStation)
        {
            _player.Play(radioStation);
            MarkStation(radioStation);
        }
    }
}
