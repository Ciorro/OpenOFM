namespace OpenOFM.Core.Settings
{
    public interface ISettingsProvider<T>
        where T : new()
    {
        event Action? SettingsSaved;

        T CurrentSettings { get; }
        void Load();
        void Save();
    }
}
