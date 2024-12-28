using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
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

        [ObservableProperty]
        private List<RadioStationItemViewModel> _radioStations = new();

        public StationsPageViewModel(IStationsStore stations, IPlaylistStore playlists, IPlayerService player)
        {
            _stations = stations;
            _playlists = playlists;
            _player = player;

            WeakReferenceMessenger.Default.Register<PlaylistsUpdatedNotification>(this, (sender, _) =>
            {
                RefreshPlaylists();
            });
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
