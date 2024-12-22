using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using OpenOFM.Core.Models;
using OpenOFM.Core.Stores;
using OpenOFM.Ui.Messages;
using OpenOFM.Ui.Navigation.Attributes;
using OpenOFM.Ui.ViewModels.Items;
using System.Windows.Threading;

namespace OpenOFM.Ui.ViewModels.Pages
{
    [PageKey("RadioStations")]
    internal partial class StationsPageViewModel : BasePageViewModel, IRecipient<RadioStationChangedMessage>
    {
        private readonly IStationsStore _stations;
        private readonly IPlaylistStore _playlists;
        private readonly DispatcherTimer _refreshTimer;

        [ObservableProperty]
        private List<RadioStationItemViewModel> _radioStations = new();

        public StationsPageViewModel(IStationsStore stations, IPlaylistStore playlists)
        {
            _stations = stations;
            _playlists = playlists;

            _refreshTimer = new DispatcherTimer();
            _refreshTimer.Interval = TimeSpan.FromSeconds(15);
            _refreshTimer.Tick += (_, __) =>
            {
                RefreshPlaylists();
            };
            _refreshTimer.Start();

            WeakReferenceMessenger.Default.Register(this);
        }

        public override void OnResumed()
        {
            PopulateStations();
            RefreshPlaylists();
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
            var stations = _stations.GetAllRadioStations().Select(x =>
            {
                var vm = new RadioStationItemViewModel(x);
                vm.Selected += OnRadioStationSelected;
                return vm;
            });

            RadioStations = new(stations);
        }

        private void RefreshPlaylists()
        {
            foreach (var station in RadioStations!)
            {
                station.Playlist = _playlists.GetPlaylist(station.Id, DateTime.Now);
            }
        }

        private void OnRadioStationSelected(object sender, RadioStation radioStation)
        {
            WeakReferenceMessenger.Default.Send(new RadioStationChangedMessage(radioStation));
        }
    }
}
