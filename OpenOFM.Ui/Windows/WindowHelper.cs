using Microsoft.Win32;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Shell;


namespace OpenOFM.Ui.Windows
{
    using static OpenOFM.Ui.Native;

    class WindowHelper
    {
        private static bool IsWindows11OrNewer
        {
            get => Environment.OSVersion.Version.Major == 10 && Environment.OSVersion.Version.Build >= 22000;
        }

        public static void ExtendGlassFrame(Window window)
        {
            nint hWnd = new WindowInteropHelper(window).EnsureHandle();

            if (window.FindName("PART_ContentWrapper") is null)
            {
                var contentWrapper = new ContentControl();
                contentWrapper.Name = "PART_ContentWrapper";
                contentWrapper.Content = window.Content;
                window.Content = contentWrapper;

                window.SizeChanged += (_, __) =>
                {
                    if (window.Content is FrameworkElement content)
                    {
                        content.Margin = new Thickness(window.WindowState == WindowState.Maximized ? 8 : 0);
                    }
                };
            }

            WindowChrome.SetWindowChrome(window, GetOSWindowChrome());

            if (IsWindows11OrNewer)
            {
                uint captionColor = 0xFFFFFFFE;
                DwmSetWindowAttribute(
                    hWnd: hWnd,
                    dwAttribute: DWMWA_CAPTION_COLOR,
                    pvAttribute: ref captionColor,
                    cbAttribute: Marshal.SizeOf<uint>()
                );
            }
            else
            {
                bool useDarkTheme = IsOSThemeDark();

                uint themeValue = useDarkTheme ? 1u : 0u;
                DwmSetWindowAttribute(
                    hWnd: hWnd,
                    dwAttribute: DWMWA_USE_IMMERSIVE_DARK_MODE,
                    pvAttribute: ref themeValue,
                    cbAttribute: Marshal.SizeOf<uint>()
                );
               
                var accentPolicy = new ACCENTPOLICY()
                {
                    nAccentState = ACCENT_ENABLE_GRADIENT,
                    nFlags = 0,
                    nColor = useDarkTheme ? 0x00040404 : 0x00EEEEEE
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
        }

        private static WindowChrome GetOSWindowChrome()
        {
            return new WindowChrome
            {
                GlassFrameThickness = new Thickness(-1),
                NonClientFrameEdges = IsWindows11OrNewer ?
                                          NonClientFrameEdges.Bottom :
                                          NonClientFrameEdges.None,
            };
        }

        private static bool IsOSThemeDark()
        {
            const string LightThemeRegistryKey = @"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize";

            using (var key = Registry.CurrentUser.OpenSubKey(LightThemeRegistryKey))
            {
                return (key?.GetValue("AppsUseLightTheme") as int?) != 1;
            }
        }
    }
}
