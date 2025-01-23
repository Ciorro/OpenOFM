using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OpenOFM.Core.Services;
using OpenOFM.Core.Settings;
using OpenOFM.Core.Stores;
using OpenOFM.Ui.Extensions;
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
                services.AddSingleton<IPlayerService, PlayerService>();
                services.AddSingleton<IPlaylistStore, PlaylistStore>();
                services.AddSingleton<IStationsStore, StationsStore>();
                services.AddSingleton<INavigationService, NavigationService>((s) =>
                {
                    return new NavigationService((pageKey) => s.GetRequiredKeyedService<IPage>(pageKey));
                });

                services.AddLocalSettings<AppSettings>("appsettings.json");
                services.AddLocalSettings<Favorites>("favorites.json");

                services.AddApi();
                services.AddPages();

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
                services.AddHostedService<StationsService>();

            }).Build();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            _appHost.Start();
            _appHost.Services.GetRequiredService<Window>().Show();

            base.OnStartup(e);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            _appHost.Dispose();
            base.OnExit(e);
        }

        private bool IsWindows11OrNewer()
        {
            return Environment.OSVersion.Version.Major == 10 && Environment.OSVersion.Version.Build >= 22000;
        }
    }

}
