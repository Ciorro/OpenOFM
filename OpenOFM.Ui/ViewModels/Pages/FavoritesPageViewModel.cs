using CommunityToolkit.Mvvm.ComponentModel;
using OpenOFM.Core.Settings;
using OpenOFM.Ui.Navigation.Attributes;
using OpenOFM.Ui.ViewModels.Items;
using System.ComponentModel;
using System.Windows.Data;

namespace OpenOFM.Ui.ViewModels.Pages
{
    [PageKey("Favorites")]
    internal partial class FavoritesPageViewModel : BasePageViewModel
    {
        private readonly ISettingsProvider<Favorites> _favorites;

        [ObservableProperty]
        private ICollectionView? _favoriteSongs;

        public FavoritesPageViewModel(ISettingsProvider<Favorites> favorites)
        {
            _favorites = favorites;
        }

        public string SongFilter
        {
            set
            {
                FavoriteSongs!.Filter = GetSongFilter(value ?? "");
            }
        }

        public override void OnResumed()
        {
            var songItems = _favorites.CurrentSettings.FavoriteSongs
                .Select(x => new SongItemViewModel(x)
                {
                    IsFavorite = true,
                    IsFavoriteChanged = OnIsFavoriteChanged
                });

            FavoriteSongs = CollectionViewSource.GetDefaultView(songItems);
        }

        private void OnIsFavoriteChanged(SongItemViewModel sender, bool isFavorite)
        {
            if (isFavorite)
            {
                _favorites.CurrentSettings.FavoriteSongs.Add(sender.Song);
            }
            else
            {
                _favorites.CurrentSettings.FavoriteSongs.Remove(sender.Song);
            }

            _favorites.Save();
        }

        private Predicate<object> GetSongFilter(string filter)
        {
            return (obj) =>
            {
                var song = (obj as SongItemViewModel)!;

                return song.Title.Contains(filter, StringComparison.CurrentCultureIgnoreCase) ||
                       song.Artist.Contains(filter, StringComparison.CurrentCultureIgnoreCase);
            };
        }
    }
}
