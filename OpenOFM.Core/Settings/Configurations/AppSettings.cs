namespace OpenOFM.Core.Settings.Configurations
{
    public class AppSettings
    {
        public string ThemeMode { get; set; } = "System";
        public float Volume { get; set; } = 100f;
        public bool IsMuted { get; set; }
        public TimeSpan MaxDelay { get; set; } = TimeSpan.FromHours(1);
    }
}
