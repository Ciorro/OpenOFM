using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Shell;

namespace OpenOFM.Ui.Windows
{
    class WindowHelper
    {
        public const int DWMWA_CAPTION_COLOR = 35;

        [DllImport("dwmapi.dll")]
        public static extern int DwmSetWindowAttribute(nint hWnd, int dwAttribute, ref uint pvAttribute, int cbAttribute);


        public static void ExtendGlassFrame(Window window)
        {
            nint hWnd = new WindowInteropHelper(window).EnsureHandle();

            uint captionColor = 0xFFFFFFFE;
            DwmSetWindowAttribute(
                hWnd: hWnd,
                dwAttribute: DWMWA_CAPTION_COLOR,
                pvAttribute: ref captionColor,
                cbAttribute: Marshal.SizeOf<uint>()
            );

            WindowChrome.SetWindowChrome(window, new WindowChrome
            {
                GlassFrameThickness = new Thickness(-1),
                NonClientFrameEdges = NonClientFrameEdges.Bottom
            });

            var contentWrapper = new ContentControl();
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
    }
}
