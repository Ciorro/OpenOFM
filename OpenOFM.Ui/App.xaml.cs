using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OpenOFM.Core.Api;
using OpenOFM.Core.Services;
using OpenOFM.Core.Settings;
using OpenOFM.Core.Settings.Configurations;
using OpenOFM.Core.Stores;
using OpenOFM.Ui.Extensions;
using OpenOFM.Ui.Navigation;
using OpenOFM.Ui.Services;
using OpenOFM.Ui.ViewModels;
using OpenOFM.Ui.ViewModels.Items;
using OpenOFM.Ui.Windows;
using System.Windows;

namespace OpenOFM.Ui
{
    public partial class App : Application
    {
        private readonly IHost _appHost;

        public App()
        {
            _appHost = Host.CreateDefaultBuilder().ConfigureServices(services =>
            {
                services.AddSingleton<IPlayerService, PlayerService>();
                services.AddSingleton<IPlaylistStore, PlaylistStore>();
                services.AddSingleton<IStationsStore, StationsStore>();
                services.AddSingleton<IRecommendationService, RecommendationService>();
                services.AddSingleton<IFeaturedService, FeaturedService>();
                services.AddSingleton<INavigationService, NavigationService>((s) =>
                {
                    return new NavigationService((pageKey) => s.GetRequiredKeyedService<IPage>(pageKey));
                });

                services.AddLocalSettings<AppSettings>("appsettings.json");
                services.AddLocalSettings<Favorites>("favorites.json");

                services.AddApi();
                services.AddPages();

                services.AddFactory<RadioStationItemViewModel>();

                services.AddSingleton<ApplicationViewModel>();
                services.AddSingleton<Window>((s) =>
                {
                    var dataContext = s.GetRequiredService<ApplicationViewModel>();
                    var settings = s.GetRequiredService<ISettingsProvider<AppSettings>>();

                    return IsWindows11OrNewer() ?
                        new MicaWindow(settings) { DataContext = dataContext } :
                        new AcrylicWindow(settings) { DataContext = dataContext };
                });

                services.AddHostedService<PlaylistService>();

            }).Build();
        }

        protected override async void OnStartup(StartupEventArgs e)
        {
            //TODO: Async radio station providers and loading indicators.
            await LoadRadioStations();
            await LoadPlaylists();

            _appHost.Start();
            _appHost.Services.GetRequiredService<Window>().Show();

            base.OnStartup(e);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            _appHost.Services.GetRequiredService<ISettingsProvider<AppSettings>>().Save();
            _appHost.Services.GetRequiredService<ISettingsProvider<Favorites>>().Save();
            _appHost.Dispose();

            base.OnExit(e);
        }

        private bool IsWindows11OrNewer()
        {
            return Environment.OSVersion.Version.Major == 10 && Environment.OSVersion.Version.Build >= 22000;
        }

        private async Task LoadRadioStations()
        {
            var stationsStore = _appHost.Services.GetRequiredService<IStationsStore>();
            var stationsApi = _appHost.Services.GetRequiredService<StationsApiClient>();

            try
            {
                var stations = await stationsApi.GetRadioStations();

                foreach (var station in stations)
                {
                    stationsStore.AddStation(station);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(
                    "Nie udało się załadować stacji radiowych.",
                    "Błąd",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                Application.Current.Shutdown();
            }
        }

        private async Task LoadPlaylists()
        {
            var playlistStore = _appHost.Services.GetRequiredService<IPlaylistStore>();
            var playlistApi = _appHost.Services.GetRequiredService<PlaylistApiClient>();

            try
            {
                var playlists = await playlistApi.GetPlaylists();

                foreach (var playlist in playlists)
                {
                    playlistStore.AddPlaylist(playlist);
                }
            }
            catch { }
        }
    }

}
