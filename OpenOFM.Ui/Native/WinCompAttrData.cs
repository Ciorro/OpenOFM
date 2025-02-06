using System.Runtime.InteropServices;

namespace OpenOFM.Ui.Native
{
    [StructLayout(LayoutKind.Sequential)]
    public struct WINCOMPATTRDATA
    {
        public int nAttribute;
        public nint pData;
        public int cbSize;
    }
}
