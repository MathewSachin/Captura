using System;
using Screna;
using System.Drawing;

namespace Captura.Models
{
    public class WindowItem : NotifyPropertyChanged, IVideoItem
    {
        public IWindow Window { get; }

        public WindowItem(IWindow Window)
        {
            this.Window = Window;
            Name = Window.Title;
        }

        public override string ToString() => Name;

        public string Name { get; }

        public IImageProvider GetImageProvider(bool IncludeCursor, out Func<Point, Point> Transform)
        {
            if (!Window.IsAlive)
            {
                throw new ObjectDisposedException(nameof(Window));
            }

            return new WindowProvider(Window, IncludeCursor, out Transform);
        }
    }
}