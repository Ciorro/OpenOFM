using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OpenOFM.Core.Models;
using OpenOFM.Core.Services.Player;
using OpenOFM.Core.Services.Playlists;
using OpenOFM.Core.Services.Stations;
using OpenOFM.Ui.Navigation.Attributes;
using OpenOFM.Ui.ViewModels.Items;
using System.Windows;
using System.Windows.Data;

namespace OpenOFM.Ui.ViewModels.Pages
{
    [PageKey("RadioStations")]
    internal partial class StationsPageViewModel : BasePageViewModel
    {
        private readonly IStationsProvider _stationsProvider;
        private readonly IPlayerService _playerService;
        private readonly IPlaylistService _playlistService;

        private CancellationTokenSource? _loadingCancellationToken;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(RadioStationCategories))]
        private List<RadioStationItemViewModel>? _radioStations;

        public StationsPageViewModel(
            IStationsProvider stationsProvider,
            IPlayerService playerService,
            IPlaylistService playlistService)
        {
            _stationsProvider = stationsProvider;
            _playlistService = playlistService;

            _playerService = playerService;
            _playerService.StationChanged += (sender, station) =>
            {
                OnPropertyChanged(nameof(SelectedStation));
            };
        }

        public RadioStationItemViewModel? SelectedStation
        {
            get => RadioStations?.SingleOrDefault(x => x.Id == _playerService.CurrentStation?.Id);
            set
            {
                if (value?.Equals(_playerService.CurrentStation) == false)
                {
                    _playerService.Play(value.Station);
                }
            }
        }

        public IEnumerable<RadioCategory>? RadioStationCategories
        {
            get => RadioStations?.SelectMany(x => x.Categories).Distinct();
        }

        public IEnumerable<object> SelectedCategories
        {
            set
            {
                var collectionView = CollectionViewSource.GetDefaultView(RadioStations);
                var selectedCategories = value.Select(x => (RadioCategory)x);

                if (selectedCategories.Any())
                {
                    collectionView.Filter = (item) =>
                    {
                        return (item as RadioStationItemViewModel)?.Categories
                            .Intersect(selectedCategories).Any() ?? false;
                    };
                }
                else
                {
                    collectionView.Filter = null;
                }

                collectionView.Refresh();
            }
        }

        public override async void OnResumed()
        {
            try
            {
                _loadingCancellationToken = new CancellationTokenSource();
                await PopulateStations(_loadingCancellationToken.Token);
            }
            catch (OperationCanceledException) { }
            catch (Exception e)
            {
                //TODO: Log
                Console.WriteLine(e.ToString());
            }
        }

        public override void OnPaused()
        {
            _loadingCancellationToken?.Cancel();
        }

        private async Task PopulateStations(CancellationToken ct)
        {
            var stationsModels = (await _stationsProvider.GetStations(ct))
                .Select(x => new RadioStationItemViewModel(x));

            Application.Current.Dispatcher.Invoke(() =>
            {
                RadioStations = stationsModels.ToList();
                UpdatePlaylists();
            });
        }

        [RelayCommand]
        private void UpdatePlaylists()
        {
            foreach (var item in RadioStations ?? [])
            {
                item.Playlist = _playlistService.GetPlaylist(item.Id, DateTime.Now);
            }
        }
    }
}
