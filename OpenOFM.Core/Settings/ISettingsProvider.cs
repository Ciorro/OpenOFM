namespace OpenOFM.Core.Settings
{
    public interface ISettingsProvider<T>
        where T : new()
    {
        T CurrentSettings { get; }
        void Load();
        void Save();
    }
}
