namespace OpenOFM.Ui.Navigation
{
    public delegate void NavigatedDelegateHandler(object pageKey);

    internal interface INavigationService
    {
        event NavigatedDelegateHandler Navigated;
        IPage? CurrentPage { get; }
        object? CurrentPageKey { get; }

        void Navigate(object pageKey);
        void Back();
        void Forward();
    }
}
