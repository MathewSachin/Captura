using System;
using Screna;
using System.Drawing;

namespace Captura.Models
{
    public class WindowPickerItem : NotifyPropertyChanged, IVideoItem
    {
        public IVideoSourcePicker Picker { get; }

        public WindowPickerItem(IVideoSourcePicker Picker)
        {
            this.Picker = Picker;
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
            var window = Picker.PickWindow();

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