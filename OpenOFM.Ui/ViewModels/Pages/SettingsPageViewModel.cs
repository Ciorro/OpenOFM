#pragma warning disable WPF0001

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OpenOFM.Core.Settings;
using OpenOFM.Core.Settings.Configurations;
using OpenOFM.Ui.Navigation.Attributes;
using System.Diagnostics;
using System.Windows;

namespace OpenOFM.Ui.ViewModels.Pages
{
    [PageKey("Settings")]
    internal partial class SettingsPageViewModel : BasePageViewModel
    {
        private readonly ISettingsProvider<AppSettings> _settings;

        [ObservableProperty]
        private ThemeMode _theme = ThemeMode.System;

        public SettingsPageViewModel(ISettingsProvider<AppSettings> settings)
        {
            _settings = settings;
        }

        public override void OnResumed()
        {
            Theme = new ThemeMode(_settings.CurrentSettings.ThemeMode);
        }

        public override void OnPaused()
        {
            _settings.Save();
        }

        [RelayCommand]
        private void OpenLink(string link)
        {
            Process.Start(new ProcessStartInfo(link)
            {
                UseShellExecute = true
            });
        }

        partial void OnThemeChanged(ThemeMode value)
        {
            Application.Current.ThemeMode = value;
            _settings.CurrentSettings.ThemeMode = value.ToString();
        }
    }
}
