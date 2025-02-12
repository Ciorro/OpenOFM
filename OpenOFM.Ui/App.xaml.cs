using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OpenOFM.Core.Api;
using OpenOFM.Core.Models;
using OpenOFM.Core.Services.Player;
using OpenOFM.Core.Services.Playlists;
using OpenOFM.Core.Services.Stations;
using OpenOFM.Core.Settings;
using OpenOFM.Core.Settings.Configurations;
using OpenOFM.Core.Stores;
using OpenOFM.Ui.Extensions;
using OpenOFM.Ui.Input;
using OpenOFM.Ui.Navigation;
using OpenOFM.Ui.Services;
using OpenOFM.Ui.ViewModels;
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
                services.AddSingleton<IPlaylistService, PlaylistService>();
                services.AddSingleton<IPlayerService, PlayerService>();
                services.AddSingleton<INavigationService, NavigationService>((s) =>
                {
                    return new NavigationService((pageKey) => s.GetRequiredKeyedService<IPage>(pageKey));
                });

                services.AddLocalSettings<AppSettings>("appsettings.json");
                services.AddLocalSettings<Favorites>("favorites.json");

                services.AddApi();
                services.AddPages();

                services.AddSingleton<IStationsProvider, ApiStationsProvider>();
                services.AddKeyedSingleton<IStationsProvider, RecommendedStationsProvider>(StationProviderKey.Recommended);
                services.AddKeyedSingleton<IStationsProvider, FeaturedStationsProvider>(StationProviderKey.Featured);

                services.AddSingleton<IStore<IReadOnlyCollection<Playlist>>, Store<IReadOnlyCollection<Playlist>>>();

                services.AddSingleton<ApplicationViewModel>();
                services.AddSingleton<Window>((s) =>
                {
                    var dataContext = s.GetRequiredService<ApplicationViewModel>();
                    var settings = s.GetRequiredService<ISettingsProvider<AppSettings>>();

                    return IsWindows11OrNewer() ?
                        new MicaWindow(settings) { DataContext = dataContext } :
                        new AcrylicWindow(settings) { DataContext = dataContext };
                });

                services.AddHostedService<PlaylistBackgroundService>();

            }).Build();
        }

        protected override async void OnStartup(StartupEventArgs e)
        {
            KeyboardListener.Start();

            //TODO: Async radio station providers and loading indicators.
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

            KeyboardListener.Stop();

            base.OnExit(e);
        }

        private bool IsWindows11OrNewer()
        {
            return Environment.OSVersion.Version.Major == 10 && Environment.OSVersion.Version.Build >= 22000;
        }

        private async Task LoadPlaylists()
        {
            var playlistStore = _appHost.Services.GetRequiredService<IStore<IReadOnlyCollection<Playlist>>>();
            var playlistApi = _appHost.Services.GetRequiredService<PlaylistApiClient>();

            try
            {
                var playlists = await playlistApi.GetPlaylists();
                playlistStore.SetValue(playlists.ToHashSet());
            }
            catch { }
        }
    }

}
