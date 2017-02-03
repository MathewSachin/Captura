using System;
using Screna;
using System.Drawing;
using WColor = System.Windows.Media.Color;

namespace Captura
{
    class WindowVSLI : IVSLI
    {
        public Window Window { get; }

        public static readonly WindowVSLI Desktop = new WindowVSLI(Window.DesktopWindow, "[Desktop]"),
            TaskBar = new WindowVSLI(Window.Taskbar, "[TaskBar]");

        public WindowVSLI(Window Window)
        {
            this.Window = Window;
            _name = Window.Title;
        }

        public WindowVSLI(Window Window, string Name)
        {
            this.Window = Window;
            _name = Name;
        }

        readonly string _name;
        
        public override string ToString() => _name;

        public IImageProvider GetImageProvider(params IOverlay[] Overlays)
        {
            Func<WColor, Color> convertColor = C => Color.FromArgb(C.A, C.R, C.G, C.B);

            return new WindowProvider(() => (App.MainViewModel.VideoViewModel.SelectedVideoSource as WindowVSLI).Window,
                    convertColor(App.MainViewModel.VideoViewModel.BackgroundColor), Overlays);
        }
    }
}