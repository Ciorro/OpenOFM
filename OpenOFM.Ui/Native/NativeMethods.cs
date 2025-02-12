using System.Runtime.InteropServices;

namespace OpenOFM.Ui.Native
{
    static class NativeMethods
    {
        public delegate nint HookProc(int nCode, nint wParam, nint lParam);

        // User32
        [DllImport("user32.dll")]
        public static extern int GetWindowLong(nint hWnd, int nIndex);

        [DllImport("user32.dll")]
        public static extern int SetWindowLong(nint hWnd, int nIndex, int dwNewLong);

        [DllImport("user32.dll")]
        public static extern int SetWindowCompositionAttribute(nint hWnd, ref WINCOMPATTRDATA pAttrData);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern nint SetWindowsHookEx(int idHook, HookProc lpfn, nint hmod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool UnhookWindowsHookEx(nint hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern nint CallNextHookEx(nint hhk, int nCode, nint wParam, nint lParam);


        // Dwmapi
        [DllImport("dwmapi.dll")]
        public static extern int DwmSetWindowAttribute(nint hWnd, int dwAttribute, ref uint pvAttribute, int cbAttribute);


        // Kernel32
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern nint GetModuleHandle(string lpModuleName);
    }
}
