using System.Runtime.InteropServices;
using System.Windows.Media;

namespace Captura
{
    public static class DWMApi
    {
        const string DllName = "dwmapi.dll";
        
        [DllImport(DllName)]
        static extern int DwmGetColorizationColor(ref int Color, [MarshalAs(UnmanagedType.Bool)] ref bool Opaque);

        [DllImport(DllName)]
        static extern int DwmIsCompositionEnabled(out bool Enabled);
        
        public static Color ColorizationColor
        {
            get
            {
                bool dwm;

                DwmIsCompositionEnabled(out dwm);

                if (!dwm)
                    return Colors.DarkBlue;

                var color = 0;
                var opaque = true;

                DwmGetColorizationColor(ref color, ref opaque);

                var c = System.Drawing.Color.FromArgb(color);

                return  Color.FromArgb(c.A, c.R, c.G, c.B);
            }
        }
    }
}
