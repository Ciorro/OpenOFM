namespace OpenOFM.Core.Stores
{
    public class Store<T> : IStore<T>
    {
        public event StoreValueChangedHandler<T>? ValueChanged;
        public T? Value { get; private set; }

        public void SetValue(T? item)
        {
            if (Value is null && item is not null)
            {
                Value = item;
                ValueChanged?.Invoke(this, Value);
            }
            else if (Value?.Equals(item) == false)
            {
                Value = item;
                ValueChanged?.Invoke(this, Value);
            }
        }
    }
}
