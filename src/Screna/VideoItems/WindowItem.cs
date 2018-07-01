using System;
using Screna;
using System.Drawing;

namespace Captura.Models
{
    public class WindowItem : NotifyPropertyChanged, IVideoItem
    {
        public Window Window { get; }

        public static readonly WindowItem TaskBar = new WindowItem(Window.Taskbar, "[TaskBar]");

        public WindowItem(Window Window)
        {
            this.Window = Window;
            Name = Window.Title;
        }

        public WindowItem(Window Window, string Name)
        {
            this.Window = Window;
            this.Name = Name;
        }

        public override string ToString() => Name;

        public string Name { get; }

        public IImageProvider GetImageProvider(bool IncludeCursor, out Func<Point, Point> Transform)
        {
            return new WindowProvider(Window, IncludeCursor, out Transform);
        }
    }
}