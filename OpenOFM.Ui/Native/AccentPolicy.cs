using System.Runtime.InteropServices;

namespace OpenOFM.Ui.Native
{
    [StructLayout(LayoutKind.Sequential)]
    public struct ACCENTPOLICY
    {
        public int nAccentState;
        public int nFlags;
        public int nColor;
        public int nAnimationId;
    }
}
