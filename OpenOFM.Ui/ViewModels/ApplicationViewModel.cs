using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OpenOFM.Ui.Navigation;

namespace OpenOFM.Ui.ViewModels
{
    internal partial class ApplicationViewModel : ObservableObject
    {
        private readonly INavigationService _navigation;

        public ApplicationViewModel(INavigationService navigation)
        {
            _navigation = navigation;
            _navigation.Navigated += (pageKey) =>
            {
                OnPropertyChanged(nameof(CurrentPage));
                OnPropertyChanged(nameof(CurrentPageKey));
            };
            _navigation.Navigate("Home");
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
    }
}
