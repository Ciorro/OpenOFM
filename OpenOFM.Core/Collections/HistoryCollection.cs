using System.Diagnostics.CodeAnalysis;

namespace OpenOFM.Core.Collections
{
    public class HistoryCollection<T>
    {
        private readonly List<T> _items = new();
        private int _currentItemIndex = -1;

        public T? CurrentItem
        {
            get => _items.ElementAtOrDefault(_currentItemIndex);
        }

        public T Back()
        {
            if (!TryBack(out var item))
            {
                throw new ArgumentOutOfRangeException();
            }

            return item;
        }

        public bool TryBack([MaybeNullWhen(false)] out T item)
        {
            if (_currentItemIndex < 1)
            {
                item = default;
                return false;
            }

            item = _items[--_currentItemIndex];
            return true;
        }

        public T Forward()
        {
            if (!TryForward(out var item))
            {
                throw new ArgumentOutOfRangeException();
            }

            return item;
        }

        public bool TryForward([MaybeNullWhen(false)] out T item)
        {
            if (_currentItemIndex >= _items.Count - 1)
            {
                item = default;
                return false;
            }

            item = _items[++_currentItemIndex];
            return true;
        }

        public void Push(T item)
        {
            while (_items.Count - 1 > _currentItemIndex)
            {
                _items.RemoveAt(_currentItemIndex + 1);
            }
            _items.Add(item);
            Forward();
        }
    }
}
