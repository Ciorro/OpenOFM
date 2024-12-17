namespace OpenOFM.Ui.Navigation
{
    internal class NavigationService : INavigationService
    {
        private readonly Func<object, IPage?> _pageFactory;
        private readonly List<object> _keys = new List<object>();
        private int _currentPageIndex = -1;

        public event NavigatedDelegateHandler? Navigated;
        public IPage? CurrentPage { get; private set; }

        public NavigationService(Func<object, IPage?> pageFactory)
        {
            _pageFactory = pageFactory;
        }

        public object? CurrentPageKey
        {
            get => _keys.ElementAtOrDefault(_currentPageIndex);
        }

        public void Navigate(object pageKey)
        {
            while (_keys.Count - 1 > _currentPageIndex)
            {
                _keys.RemoveAt(_currentPageIndex + 1);
            }
            _keys.Add(pageKey);

            Forward();
        }

        public void Back()
        {
            if (_currentPageIndex < 1)
                return;

            CurrentPage?.OnPaused();
            CurrentPage = _pageFactory(_keys[--_currentPageIndex]);
            CurrentPage?.OnResumed();

            Navigated?.Invoke(CurrentPage!);
        }

        public void Forward()
        {
            if (_currentPageIndex >= _keys.Count - 1)
                return;

            CurrentPage?.OnPaused();
            CurrentPage = _pageFactory(_keys[++_currentPageIndex]);
            CurrentPage?.OnResumed();

            Navigated?.Invoke(CurrentPage!);
        }
    }
}
