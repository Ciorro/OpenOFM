using CommunityToolkit.Mvvm.ComponentModel;
using OpenOFM.Ui.Navigation;

namespace OpenOFM.Ui.ViewModels.Pages
{
    internal abstract partial class BasePageViewModel : ObservableObject, IPage
    {
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(HasError))]
        private string? _error;

        public bool HasError
        {
            get => Error is not null;
        }

        public virtual void OnResumed() { }
        public virtual void OnPaused() { }
    }
}
