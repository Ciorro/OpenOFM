using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OpenOFM.Core.Models;
using System.Text;

namespace OpenOFM.Ui.ViewModels.Items
{
    internal partial class RadioStationItemViewModel : ObservableObject
    {
        private readonly RadioStation _radioStation;

        [ObservableProperty]
        private bool _isPlaying;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(CurrentSong))]
        private Playlist? _playlist;

        public Action<RadioStation>? OnSelected { get; set; }

        public RadioStationItemViewModel(RadioStation radioStation)
        {
            _radioStation = radioStation;
        }

        public int Id
            => _radioStation.Id;
        public string? Name
            => _radioStation.Name;
        public string? CoverUrl
            => _radioStation.CoverUrl;
        public IReadOnlyList<RadioCategory> Categories
            => _radioStation.Categories;

        public string CurrentSong
        {
            get
            {
                var currentSong = Playlist?.Queue.ElementAtOrDefault(0);
                var isFullTitle = !string.IsNullOrWhiteSpace(currentSong?.Title) &&
                                  !string.IsNullOrWhiteSpace(currentSong?.Artist);

                var titleBuilder = new StringBuilder();
                titleBuilder.Append(currentSong?.Title);
                if (isFullTitle)
                {
                    titleBuilder.Append(" - ");
                }
                titleBuilder.Append(currentSong?.Artist);

                return titleBuilder.ToString();
            }
        }

        [RelayCommand]
        private void OnClick()
        {
            OnSelected?.Invoke(_radioStation);
        }
    }
}
