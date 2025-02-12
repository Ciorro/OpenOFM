using CommunityToolkit.Mvvm.ComponentModel;
using OpenOFM.Core.Models;
using System.Text;

namespace OpenOFM.Ui.ViewModels.Items
{
    internal partial class RadioStationItemViewModel : ObservableObject, IEquatable<RadioStationItemViewModel>
    {
        public RadioStation Station { get; }

        [ObservableProperty, NotifyPropertyChangedFor(nameof(CurrentSong))]
        private Playlist? _playlist;

        public RadioStationItemViewModel(RadioStation station)
        {
            Station = station;
        }

        public int Id
            => Station.Id;
        public string? Name
            => Station.Name;
        public string? CoverUrl
            => Station.CoverUrl;
        public IReadOnlyList<RadioCategory> Categories
            => Station.Categories;

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

        public bool Equals(RadioStationItemViewModel? other)
        {
            return other?.Id == Id;
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as RadioStationItemViewModel);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public static bool operator ==(RadioStationItemViewModel? x, RadioStationItemViewModel? y)
        {
            return x?.Equals(y) == true;
        }

        public static bool operator !=(RadioStationItemViewModel? x, RadioStationItemViewModel? y)
        {
            return !(x == y);
        }
    }
}
