using Screna;
using System;

namespace Captura
{
    class WindowVSLI : IVideoSourceListItem
    {
        public IntPtr Handle { get; }

        public static readonly WindowVSLI Desktop = new WindowVSLI(WindowProvider.DesktopHandle, "[Desktop]"),
            TaskBar = new WindowVSLI(WindowProvider.TaskbarHandle, "[TaskBar]");

        public WindowVSLI(IntPtr hWnd)
        {
            Handle = hWnd;
            Name = new WindowHandler(Handle).Title;
        }

        public WindowVSLI(IntPtr hWnd, string Name)
        {
            Handle = hWnd;
            this.Name = Name;
        }

        public string Name { get; }
    }
}