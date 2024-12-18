using CommunityToolkit.Mvvm.ComponentModel;

namespace OpenOFM.Ui.ViewModels
{
    partial class MediaControlsViewModel : ObservableObject
    {
        [ObservableProperty]
        private bool _isPlaying;

        [ObservableProperty]
        private bool _isMuted;

        [ObservableProperty]
        private float _volume;

        public TimeSpan Delay
        {
            get => TimeSpan.FromSeconds(5);
        }
    }
}
