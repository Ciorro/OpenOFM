using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using OpenOFM.Core.Services.Player;
using OpenOFM.Core.Services.Playlists;
using OpenOFM.Core.Services.Stations;
using OpenOFM.Ui.Navigation.Attributes;
using OpenOFM.Ui.ViewModels.Items;
using System.Windows;

namespace OpenOFM.Ui.ViewModels.Pages
{
    [PageKey("Home")]
    internal partial class HomePageViewModel : BasePageViewModel
    {
        private readonly IStationsProvider _featuredStationsProvider;
        private readonly IStationsProvider _recommendedStationsProvider;
        private readonly IPlaylistService _playlistService;
        private readonly IPlayerService _playerService;

        private CancellationTokenSource? _loadingCancellationToken;

        private RadioStationItemViewModel? _selectedStation;

        [ObservableProperty]
        private List<RadioStationItemViewModel>? _featuredStations;

        [ObservableProperty]
        private List<RadioStationItemViewModel>? _recommendedStations;

        public HomePageViewModel(
            [FromKeyedServices(StationProviderKey.Featured)] IStationsProvider featuredStationsProvider,
            [FromKeyedServices(StationProviderKey.Recommended)] IStationsProvider recommendedStationsProvider,
            IPlaylistService playlistService,
            IPlayerService playerService)
        {
            _featuredStationsProvider = featuredStationsProvider;
            _recommendedStationsProvider = recommendedStationsProvider;
            _playlistService = playlistService;

            _playerService = playerService;
            _playerService.StationChanged += (sender, station) =>
            {
                var currentStationModel = station is not null ?
                    new RadioStationItemViewModel(station) : null;
                SelectStation(currentStationModel);
            };
        }

        public RadioStationItemViewModel? SelectedFeaturedStation
        {
            get => FeaturedStations?.FirstOrDefault(x => x.Equals(_selectedStation));
            set
            {
                if (value?.Equals(_selectedStation) == false)
                {
                    _playerService.Play(value.Station);
                }
                SelectStation(value);
            }
        }

        public RadioStationItemViewModel? SelectedRecommendedStation
        {
            get => RecommendedStations?.FirstOrDefault(x => x.Equals(_selectedStation));
            set
            {
                if (value?.Equals(_selectedStation) == false)
                {
                    _playerService.Play(value.Station);
                }
                SelectStation(value);
            }
        }

        public override async void OnResumed()
        {
            try
            {
                _loadingCancellationToken = new CancellationTokenSource();
                await Task.WhenAll(
                    PopulateFeatured(_loadingCancellationToken.Token),
                    PopulateRecommended(_loadingCancellationToken.Token));

                var currentStationModel = _playerService.CurrentStation is not null ?
                    new RadioStationItemViewModel(_playerService.CurrentStation) :
                    null;
                SelectStation(currentStationModel);
                UpdatePlaylists();
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

        private async Task PopulateFeatured(CancellationToken ct)
        {
            var featuredStationsModels = (await _featuredStationsProvider.GetStations(ct))
                .Select(x => new RadioStationItemViewModel(x));

            Application.Current.Dispatcher.Invoke(() =>
            {
                FeaturedStations = featuredStationsModels.ToList();
            });
        }

        private async Task PopulateRecommended(CancellationToken ct)
        {
            var recommendedStationsModels = (await _recommendedStationsProvider.GetStations(ct))
                .Select(x => new RadioStationItemViewModel(x));

            Application.Current.Dispatcher.Invoke(() =>
            {
                RecommendedStations = recommendedStationsModels.ToList();
            });
        }

        [RelayCommand]
        private void UpdatePlaylists()
        {
            foreach (var item in RecommendedStations?.Concat(FeaturedStations ?? []) ?? [])
            {
                item.Playlist = _playlistService.GetPlaylist(item.Id, DateTime.Now);
            }
        }

        private void SelectStation(RadioStationItemViewModel? stationViewModel)
        {
            _selectedStation = stationViewModel;
            OnPropertyChanged(nameof(SelectedFeaturedStation));
            OnPropertyChanged(nameof(SelectedRecommendedStation));
        }
    }
}
