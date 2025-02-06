#pragma warning disable WPF0001

using OpenOFM.Core.Settings;
using OpenOFM.Core.Settings.Configurations;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Shell;
using static OpenOFM.Ui.Native.NativeConstants;
using static OpenOFM.Ui.Native.NativeMethods;

namespace OpenOFM.Ui.Windows
{
    public partial class MicaWindow : Window
    {
        public MicaWindow(ISettingsProvider<AppSettings> settings)
        {
            InitializeComponent();
            ExtendGlassFrame();

            Application.Current.ThemeMode = new ThemeMode(settings.CurrentSettings.ThemeMode);
        }

        private void ExtendGlassFrame()
        {
            nint hWnd = new WindowInteropHelper(this).EnsureHandle();

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
                NonClientFrameEdges = NonClientFrameEdges.Bottom
            });

            uint captionColor = 0xFFFFFFFE;
            DwmSetWindowAttribute(
                hWnd: hWnd,
                dwAttribute: DWMWA_CAPTION_COLOR,
                pvAttribute: ref captionColor,
                cbAttribute: Marshal.SizeOf<uint>()
            );
        }
    }
}
