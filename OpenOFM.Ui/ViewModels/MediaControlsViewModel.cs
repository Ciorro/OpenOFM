using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using OpenOFM.Core.Services;
using OpenOFM.Ui.Messages;
using System.Windows.Threading;

namespace OpenOFM.Ui.ViewModels
{
    partial class MediaControlsViewModel : ObservableObject, IRecipient<RadioStationChangedMessage>
    {
        private readonly IPlayerService _playerService;

        [ObservableProperty]
        private bool _isPaused;

        [ObservableProperty]
        private bool _isMuted;

        [ObservableProperty]
        private float _volume = 100;

        [ObservableProperty]
        private TimeSpan _delay;

        public MediaControlsViewModel(IPlayerService playerService)
        {
            _playerService = playerService;

            var delayRefreshTimer = new DispatcherTimer();
            delayRefreshTimer.Interval = TimeSpan.FromSeconds(1);
            delayRefreshTimer.Tick += (_, __) =>
            {
                Delay = _playerService.Delay;
            };
            delayRefreshTimer.Start();

            WeakReferenceMessenger.Default.Register(this);
        }

        public async void Receive(RadioStationChangedMessage message)
        {
            try
            {
                await _playerService.Play(message.Value);
            }
            catch (InvalidOperationException) { }

            IsPaused = false;
            Delay = TimeSpan.Zero;
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
        }

        partial void OnVolumeChanged(float value)
        {
            if (!IsMuted)
            {
                _playerService.Volume = value / 100f;
            }
        }
    }
}
