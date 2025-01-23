using CommunityToolkit.Mvvm.ComponentModel;
using OpenOFM.Core.Models;

namespace OpenOFM.Ui.ViewModels.Items
{
    internal partial class SongItemViewModel : ObservableObject
    {
        public Song Song { get; }
        public Action<SongItemViewModel, bool>? IsFavoriteChanged { get; set; }

        [ObservableProperty]
        private bool _isFavorite;

        public SongItemViewModel(Song song)
        {
            Song = song;
        }

        public string Title
        {
            get => Song.Title;
        }

        public string Artist
        {
            get => Song.Artist;
        }

        partial void OnIsFavoriteChanged(bool value)
        {
            IsFavoriteChanged?.Invoke(this, value);
        }
    }
}
