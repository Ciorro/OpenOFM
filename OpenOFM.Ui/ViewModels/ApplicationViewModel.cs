using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OpenOFM.Core.Models;
using OpenOFM.Core.Services.Player;
using OpenOFM.Core.Settings;
using OpenOFM.Core.Settings.Configurations;
using OpenOFM.Ui.Input;
using OpenOFM.Ui.Navigation;
using System.Windows.Input;

namespace OpenOFM.Ui.ViewModels
{
    internal partial class ApplicationViewModel : ObservableObject
    {
        private readonly INavigationService _navigation;

        [ObservableProperty]
        private RadioStation? _currentStation;

        [ObservableProperty]
        private MediaControlsViewModel? _mediaControlsViewModel;

        public ApplicationViewModel(INavigationService navigation, IPlayerService playerService, ISettingsProvider<AppSettings> settings)
        {
            _navigation = navigation;
            _navigation.Navigated += (pageKey) =>
            {
                OnPropertyChanged(nameof(CurrentPage));
                OnPropertyChanged(nameof(CurrentPageKey));
            };
            _navigation.Navigate("Home");

            playerService.StationChanged += (sender, station) =>
            {
                CurrentStation = station;

                if (station is not null)
                {
                    MediaControlsViewModel = new MediaControlsViewModel(playerService, settings);
                }
                else
                {
                    MediaControlsViewModel = null;
                }
            };

            KeyboardListener.KeyPressed += OnGlobalKeyPressed;
        }

        public IPage? CurrentPage
        {
            get => _navigation.CurrentPage;
        }

        public object CurrentPageKey
        {
            get => _navigation.CurrentPageKey!;
            set
            {
                _navigation.Navigate(value);
            }
        }

        [RelayCommand]
        private void NavigateBack()
        {
            _navigation.Back();
        }

        [RelayCommand]
        private void NavigateForward()
        {
            _navigation.Forward();
        }

        private async void OnGlobalKeyPressed(Key key)
        {
            if (MediaControlsViewModel is not null)
            {
                try
                {
                    switch (key)
                    {
                        case Key.MediaPlayPause:
                            MediaControlsViewModel.IsPaused = !MediaControlsViewModel.IsPaused;
                            break;
                        case Key.MediaPreviousTrack:
                            await MediaControlsViewModel.PlayPreviousStationCommand.ExecuteAsync(null);
                            break;
                        case Key.MediaNextTrack:
                            await MediaControlsViewModel.PlayNextStationCommand.ExecuteAsync(null);
                            break;
                    }
                }
                catch (Exception e)
                {
                    //TODO: Log
                    Console.WriteLine(e.ToString());
                }
            }
        }
    }
}
