#pragma warning disable WPF0001

using Microsoft.Win32;
using OpenOFM.Core.Settings;
using OpenOFM.Core.Settings.Configurations;
using OpenOFM.Ui.Native;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Shell;
using static OpenOFM.Ui.Native.NativeConstants;
using static OpenOFM.Ui.Native.NativeMethods;

namespace OpenOFM.Ui.Windows
{
    public partial class AcrylicWindow : Window
    {
        public AcrylicWindow(ISettingsProvider<AppSettings> settings)
        {
            Application.Current.ThemeMode = new ThemeMode(settings.CurrentSettings.ThemeMode);

            InitializeComponent();

            nint hWnd = new WindowInteropHelper(this).EnsureHandle();
            ExtendGlassFrame(hWnd);
            SetWindowTheme(hWnd);

            settings.SettingsSaved += () =>
            {
                SetWindowTheme(hWnd);
            };
        }

        private void ExtendGlassFrame(nint hWnd)
        {
            if (FindName("PART_ContentWrapper") is null)
            {
                var contentWrapper = new ContentControl();
                contentWrapper.Name = "PART_ContentWrapper";
                contentWrapper.Content = Content;
                Content = contentWrapper;

                SizeChanged += (_, __) =>
                {
                    if (Content is FrameworkElement content)
                    {
                        content.Margin = new Thickness(WindowState == WindowState.Maximized ? 8 : 0);
                    }
                };
            }

            WindowChrome.SetWindowChrome(this, new WindowChrome
            {
                GlassFrameThickness = new Thickness(-1),
                NonClientFrameEdges = NonClientFrameEdges.None
            });
        }

        private void SetWindowTheme(nint hWnd)
        {
            bool isLigthTheme = IsLigthTheme();

            var accentPolicy = new ACCENTPOLICY()
            {
                nAccentState = ACCENT_ENABLE_GRADIENT,
                nFlags = 0,
                nColor = isLigthTheme ? 0x00EEEEEE : 0x00040404
            };

            int accentPolicySize = Marshal.SizeOf<ACCENTPOLICY>();
            nint accentPolicyPtr = Marshal.AllocHGlobal(accentPolicySize);
            Marshal.StructureToPtr(accentPolicy, accentPolicyPtr, false);

            var data = new WINCOMPATTRDATA()
            {
                nAttribute = WCA_ACCENT_POLICY,
                pData = accentPolicyPtr,
                cbSize = accentPolicySize
            };
            SetWindowCompositionAttribute(hWnd, ref data);

            Marshal.FreeHGlobal(accentPolicyPtr);
        }

        private bool IsLigthTheme()
        {
            if (Application.Current.ThemeMode == ThemeMode.Light)
            {
                return true;
            }

            if (Application.Current.ThemeMode == ThemeMode.System)
            {
                const string LightThemeRegistryKey = @"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize";

                using (var key = Registry.CurrentUser.OpenSubKey(LightThemeRegistryKey))
                {
                    return (key?.GetValue("AppsUseLightTheme") as int?) == 1;
                }
            }

            return false;
        }
    }
}
