using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OpenOFM.Core.Services;
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

                services.AddApi();
                services.AddPages();
                services.AddViewModels();

                services.AddSingleton<ApplicationViewModel>();
                services.AddSingleton<ApplicationWindow>((s) => new ApplicationWindow
                {
                    DataContext = s.GetRequiredService<ApplicationViewModel>()
                });

                services.AddHostedService<PlaylistService>();
                services.AddHostedService<StationsService>();

            }).Build();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            _appHost.Start();
            _appHost.Services.GetRequiredService<ApplicationWindow>().Show();

            base.OnStartup(e);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            _appHost.Dispose();
            base.OnExit(e);
        }
    }

}
