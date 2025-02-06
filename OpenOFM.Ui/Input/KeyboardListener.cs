using OpenOFM.Ui.Native;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Input;
using static OpenOFM.Ui.Native.NativeMethods;

namespace OpenOFM.Ui.Input
{
    static class KeyboardListener
    {
        private static HookProc _hookCallback = OnHookTriggered;
        private static nint _hookId = -1;

        public static event Action<Key>? KeyPressed;

        public static void Start()
        {
            using (var process = Process.GetCurrentProcess())
            using (var module = process.MainModule)
            {
                if (module is not null)
                {
                    _hookId = SetWindowsHookEx(
                        idHook: NativeConstants.WH_KEYBOARD_LL,
                        lpfn: _hookCallback,
                        hmod: GetModuleHandle(module.ModuleName),
                        dwThreadId: 0
                    );

                    int error = Marshal.GetLastWin32Error();
                    if (error != 0)
                    {
                        throw new Win32Exception(error);
                    }
                }
            }
        }

        public static void Stop()
        {
            if (UnhookWindowsHookEx(_hookId))
            {
                _hookId = -1;
            }
        }

        private static nint OnHookTriggered(int nCode, nint wParam, nint lParam)
        {
            int vkCode = Marshal.ReadInt32(lParam);
            int flags = Marshal.ReadInt32(lParam, 8);
            bool justPressed = (flags & 128) != 128;

            if (justPressed)
            {
                KeyPressed?.Invoke(KeyInterop.KeyFromVirtualKey(vkCode));
            }

            return CallNextHookEx(_hookId, nCode, wParam, lParam);
        }
    }
}
