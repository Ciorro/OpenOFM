using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OpenOFM.Core.Services.Player;
using OpenOFM.Core.Settings;
using OpenOFM.Core.Settings.Configurations;

namespace OpenOFM.Ui.ViewModels
{
    internal partial class MediaControlsViewModel : ObservableObject
    {
        private readonly IPlayerService _playerService;
        private readonly ISettingsProvider<AppSettings> _settings;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(TaskbarMediaControlsIcon))]
        [NotifyPropertyChangedFor(nameof(TaskbarMediaControlsDescription))]
        private bool _isPaused;

        [ObservableProperty]
        private bool _isMuted;

        [ObservableProperty]
        private float _volume;

        public TimeSpan Delay
        {
            get => _playerService.GetDelay();
        }

        public MediaControlsViewModel(IPlayerService playerService, ISettingsProvider<AppSettings> settings)
        {
            _settings = settings;
            _playerService = playerService;

            Volume = _settings.CurrentSettings.Volume;
            IsMuted = _settings.CurrentSettings.IsMuted;
        }

        public string TaskbarMediaControlsDescription
        {
            get => IsPaused ? "Odtwórz" : "Wstrzymaj";
        }

        public string TaskbarMediaControlsIcon
        {
            get => IsPaused ? "/Assets/play.png" : "/Assets/pause.png";
        }

        [RelayCommand]
        private async Task PlayPreviousStation()
        {
            await _playerService.PlayPrevious();
        }

        [RelayCommand]
        private async Task PlayNextStation()
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
