#pragma warning disable WPF0001

using CommunityToolkit.Mvvm.Input;
using OpenOFM.Core.Settings;
using OpenOFM.Core.Settings.Configurations;
using OpenOFM.Ui.Navigation.Attributes;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows;

namespace OpenOFM.Ui.ViewModels.Pages
{
    [PageKey("Settings")]
    internal partial class SettingsPageViewModel : BasePageViewModel
    {
        private readonly ISettingsProvider<AppSettings> _settings;

        public SettingsPageViewModel(ISettingsProvider<AppSettings> settings)
        {
            _settings = settings;
        }

        public ThemeMode Theme
        {
            get => new ThemeMode(_settings.CurrentSettings.ThemeMode);
            set
            {
                Application.Current.ThemeMode = value;
                _settings.CurrentSettings.ThemeMode = value.ToString();
                _settings.Save();
            }
        }

        public uint MaxDelay
        {
            get => (uint)_settings.CurrentSettings.MaxDelay.TotalHours;
            set
            {
                if (value > 0 && value <= 24)
                {
                    _settings.CurrentSettings.MaxDelay = TimeSpan.FromHours(value);
                    _settings.Save();
                }
            }
        }

        public string AppVersion
        {
            get => Assembly.GetExecutingAssembly().GetName().Version!.ToString();
        }

        public override void OnPaused()
        {
            _settings.Save();
        }

        [RelayCommand]
        private void OpenConfigFolder()
        {
            string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "OpenOFM");

            Process.Start(new ProcessStartInfo(path)
            {
                UseShellExecute = true
            });
        }

        [RelayCommand]
        private void OpenLink(string link)
        {
            Process.Start(new ProcessStartInfo(link)
            {
                UseShellExecute = true
            });
        }
    }
}
