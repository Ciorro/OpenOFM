using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using OpenOFM.Core.Models;
using OpenOFM.Core.Services;
using OpenOFM.Core.Stores;
using OpenOFM.Ui.Messages;
using System.Text;
using System.Windows;

namespace OpenOFM.Ui.ViewModels.Items
{
    internal partial class RadioStationItemViewModel : ObservableObject, IDisposable
    {
        private readonly IPlaylistStore _playlists;
        private readonly IPlayerService _player;

        public RadioStation Station { get; set; } = new();

        public RadioStationItemViewModel(IPlaylistStore playlists, IPlayerService player)
        {
            _playlists = playlists;
            _player = player;
            _player.StationChanged += OnStationChanged;

            WeakReferenceMessenger.Default.Register<PlaylistsUpdatedNotification>(this, (sender, _) =>
            {
                OnPropertyChanged(nameof(CurrentSong));
            });
        }

        public int Id
            => Station.Id;
        public string? Name
            => Station.Name;
        public string? CoverUrl
            => Station.CoverUrl;
        public IReadOnlyList<RadioCategory> Categories
            => Station.Categories;

        public bool IsPlaying
        {
            get => _player.CurrentStation == Station;
        }

        public string CurrentSong
        {
            get
            {
                var playlist = _playlists.GetPlaylist(Station.Id, DateTime.Now);
                var currentSong = playlist?.Queue.ElementAtOrDefault(0);
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
            _player.Play(Station);
        }

        private void OnStationChanged(object sender, RadioStation? station)
        {
            OnPropertyChanged(nameof(IsPlaying));
        }

        public void Dispose()
        {
            _player.StationChanged -= OnStationChanged;
        }
    }
}
