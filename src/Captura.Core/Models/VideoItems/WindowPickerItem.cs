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

        const string WindowPickerName = "Window Picker";

        string _name = WindowPickerName;

        public string Name
        {
            get => _name;
            private set
            {
                _name = value;
                
                OnPropertyChanged();
            }
        }

        public IImageProvider GetImageProvider(bool IncludeCursor, out Func<Point, Point> Transform)
        {
            var window = _picker.PickWindow();

            if (window == null)
            {
                Transform = null;

                Name = WindowPickerName;

                return null;
            }

            Name = $"{WindowPickerName} ({window.Title})";

            return new WindowProvider(window, IncludeCursor, out Transform);
        }
    }
}