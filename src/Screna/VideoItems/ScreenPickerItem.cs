using System;
using Screna;
using System.Drawing;

namespace Captura.Models
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class ScreenPickerItem : NotifyPropertyChanged, IVideoItem
    {
        public IVideoSourcePicker Picker { get; }

        readonly LanguageManager _loc;

        public ScreenPickerItem(IVideoSourcePicker Picker, LanguageManager Loc)
        {
            this.Picker = Picker;
            _loc = Loc;

            _loc.LanguageChanged += L => RaisePropertyChanged(nameof(Name));
        }

        public override string ToString() => Name;

        public string Name => _loc.ScreenPicker;

        public IImageProvider GetImageProvider(bool IncludeCursor, out Func<Point, Point> Transform)
        {
            var screen = Picker.PickScreen();

            if (screen == null)
            {
                Transform = null;

                return null;
            }

            Transform = P => new Point(P.X - screen.Rectangle.X, P.Y - screen.Rectangle.Y);

            return new RegionProvider(screen.Rectangle, IncludeCursor);
        }
    }
}