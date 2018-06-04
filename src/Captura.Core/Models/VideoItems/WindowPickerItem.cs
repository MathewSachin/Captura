using System;
using Screna;
using System.Drawing;

namespace Captura.Models
{
    public class WindowPickerItem : NotifyPropertyChanged, IVideoItem
    {
        readonly IVideoSourcePicker _picker;

        public WindowPickerItem(IVideoSourcePicker Picker)
        {
            _picker = Picker;
        }

        public override string ToString() => Name;

        public string Name { get; } = "Window Picker";

        public IImageProvider GetImageProvider(bool IncludeCursor, out Func<Point, Point> Transform)
        {
            var window = _picker.PickWindow();

            if (window == null)
            {
                Transform = null;

                return null;
            }

            return new WindowProvider(window, IncludeCursor, out Transform);
        }
    }
}