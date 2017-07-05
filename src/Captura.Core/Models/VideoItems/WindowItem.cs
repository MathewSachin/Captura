using System;
using Screna;
using System.Drawing;

namespace Captura.Models
{
    public class WindowItem : IVideoItem
    {
        public Window Window { get; }

        public static readonly WindowItem TaskBar = new WindowItem(Window.Taskbar, "[TaskBar]");

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
            
            return new WindowProvider(ServiceProvider.Get<Func<Window>>(ServiceName.SelectedWindow), Settings.Instance.VideoBackgroundColor);
        }
    }
}