using System.Runtime.InteropServices;

namespace OpenOFM.Ui
{
    internal static class Native
    {
        public const int DWMWA_USE_IMMERSIVE_DARK_MODE = 20;
        public const int DWMWA_CAPTION_COLOR = 35;
        public const int DWMWA_SYSTEM_BACKDROP_TYPE = 38;
        public const int WCA_ACCENT_POLICY = 19;
        public const int ACCENT_DISABLED = 0;
        public const int ACCENT_ENABLE_GRADIENT = 1;
        public const int ACCENT_ENABLE_TRANSPARENTGRADIENT = 2;
        public const int ACCENT_ENABLE_BLURBEHIND = 3;
        public const int ACCENT_ENABLE_ACRYLICBLURBEHIND = 4;
        public const int ACCENT_INVALID_STATE = 5;
        public const int DRAW_LEFT_BORDER = 0x20;
        public const int DRAW_TOP_BORDER = 0x40;
        public const int DRAW_RIGHT_BORDER = 0x80;
        public const int DRAW_BOTTOM_BORDER = 0x100;
        public const int DRAW_ALL_BORDERS = 0x1E0;

        [StructLayout(LayoutKind.Sequential)]
        public struct WINCOMPATTRDATA
        {
            public int nAttribute;
            public nint pData;
            public int cbSize;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct ACCENTPOLICY
        {
            public int nAccentState;
            public int nFlags;
            public int nColor;
            public int nAnimationId;
        }

        //User32
        [DllImport("user32.dll")]
        public static extern int GetWindowLong(nint hWnd, int nIndex);

        [DllImport("user32.dll")]
        public static extern int SetWindowLong(nint hWnd, int nIndex, int dwNewLong);

        [DllImport("user32.dll")]
        public static extern int SetWindowCompositionAttribute(nint hWnd, ref WINCOMPATTRDATA pAttrData);


        // Dwmapi
        [DllImport("dwmapi.dll")]
        public static extern int DwmSetWindowAttribute(nint hWnd, int dwAttribute, ref uint pvAttribute, int cbAttribute);
    }
}
