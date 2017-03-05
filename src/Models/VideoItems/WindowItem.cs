using System;
using Screna;
using System.Drawing;
using WColor = System.Windows.Media.Color;

namespace Captura
{
    class WindowItem : IVideoItem
    {
        public Window Window { get; }

        public static readonly WindowItem Desktop = new WindowItem(Window.DesktopWindow, "[Desktop]"),
            TaskBar = new WindowItem(Window.Taskbar, "[TaskBar]");

        public WindowItem(Window Window)
        {
            this.Window = Window;
            _name = Window.Title;
        }

        public WindowItem(Window Window, string Name)
        {
            this.Window = Window;
            _name = Name;
        }

        readonly string _name;
        
        public override string ToString() => _name;

        public IImageProvider GetImageProvider(out Func<Point> Offset)
        {
            Offset = () => Point.Empty;

            Func<WColor, Color> convertColor = C => Color.FromArgb(C.A, C.R, C.G, C.B);

            return new WindowProvider(() => (MainViewModel.Instance.VideoViewModel.SelectedVideoSource as WindowItem).Window,
                    convertColor(MainViewModel.Instance.VideoViewModel.BackgroundColor));
        }
    }
}