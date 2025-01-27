using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OpenOFM.Core.Collections;
using OpenOFM.Core.Models;
using OpenOFM.Core.Services;
using OpenOFM.Core.Settings;
using OpenOFM.Core.Settings.Configurations;
using OpenOFM.Ui.Navigation;
using System.Windows.Threading;

namespace OpenOFM.Ui.ViewModels
{
    internal partial class ApplicationViewModel : ObservableObject
    {
        private readonly INavigationService _navigation;
        private readonly IPlayerService _playerService;
        private readonly ISettingsProvider<AppSettings> _settings;

        [ObservableProperty]
        private bool _isPaused;

        [ObservableProperty]
        private bool _isMuted;

        [ObservableProperty]
        private float _volume = 100;

        [ObservableProperty]
        private TimeSpan _delay;

        [ObservableProperty]
        private RadioStation? _currentStation;

        public ApplicationViewModel(INavigationService navigation, IPlayerService playerService, ISettingsProvider<AppSettings> settings)
        {
            _playerService = playerService;
            _playerService.StationChanged += (sender, station) =>
            {
                IsPaused = false;
                Delay = TimeSpan.Zero;
                CurrentStation = station;
            };

            _navigation = navigation;
            _navigation.Navigated += (pageKey) =>
            {
                OnPropertyChanged(nameof(CurrentPage));
                OnPropertyChanged(nameof(CurrentPageKey));
            };
            _navigation.Navigate("Home");

            _settings = settings;
            Volume = _settings.CurrentSettings.Volume;
            IsMuted = _settings.CurrentSettings.IsMuted;

            var delayRefreshTimer = new DispatcherTimer();
            delayRefreshTimer.Interval = TimeSpan.FromSeconds(1);
            delayRefreshTimer.Tick += (_, __) =>
            {
                Delay = _playerService.GetDelay();
            };
            delayRefreshTimer.Start();
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
            => _navigation.Back();

        [RelayCommand]
        private void NavigateForward()
            => _navigation.Forward();

        [RelayCommand]
        private async Task PreviousStation()
        {
            await _playerService.PlayPrevious();
        }

        [RelayCommand]
        private async Task NextStation()
        {
            await _playerService.PlayNext();
        }

        partial void OnIsPausedChanged(bool value)
        {
            _playerService.IsPaused = value;
        }

        partial void OnIsMutedChanged(bool value)
        {
            if (value)
            {
                _playerService.Volume = 0;
            }
            else
            {
                _playerService.Volume = Volume / 100f;
            }

            _settings.CurrentSettings.IsMuted = value;
            _settings.Save();
        }

        partial void OnVolumeChanged(float value)
        {
            if (!IsMuted)
            {
                _playerService.Volume = value / 100f;
            }

            _settings.CurrentSettings.Volume = value;
        }
    }
}
