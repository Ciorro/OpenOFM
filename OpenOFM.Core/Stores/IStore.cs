namespace OpenOFM.Core.Stores
{
    public delegate void StoreValueChangedHandler<T>(object sender, T? value);

    public interface IStore<T>
    {
        event StoreValueChangedHandler<T>? ValueChanged;

        T? Value { get; }
        void SetValue(T? valule);
    }
}