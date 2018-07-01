using System;
using Screna;
using System.Drawing;

namespace Captura.Models
{
    public class ScreenPickerItem : NotifyPropertyChanged, IVideoItem
    {
        public IVideoSourcePicker Picker { get; }

        public ScreenPickerItem(IVideoSourcePicker Picker)
        {
            this.Picker = Picker;
        }

        public override string ToString() => Name;

        const string ScreenPickerName = "Screen Picker";

        string _name = ScreenPickerName;

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
            var screen = Picker.PickScreen();

            if (screen == null)
            {
                Transform = null;

                Name = ScreenPickerName;

                return null;
            }

            Name = $"{ScreenPickerName} ({screen.DeviceName})";

            Transform = P => new Point(P.X - screen.Rectangle.X, P.Y - screen.Rectangle.Y);

            return new RegionProvider(screen.Rectangle, IncludeCursor);
        }
    }
}