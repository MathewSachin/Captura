using System;
using Screna;
using System.Drawing;

namespace Captura.Models
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class WindowPickerItem : NotifyPropertyChanged, IVideoItem
    {
        public IVideoSourcePicker Picker { get; }

        readonly LanguageManager _loc;

        public WindowPickerItem(IVideoSourcePicker Picker, LanguageManager Loc)
        {
            this.Picker = Picker;
            _loc = Loc;

            _loc.LanguageChanged += L => RaisePropertyChanged(nameof(Name));
        }

        public override string ToString() => Name;
        
        public string Name => _loc.WindowPicker;

        public IImageProvider GetImageProvider(bool IncludeCursor, out Func<Point, Point> Transform)
        {
            var window = Picker.PickWindow();

            if (window == null)
            {
                Transform = null;
                
                return null;
            }

            return new WindowProvider(window, IncludeCursor, out Transform);
        }
    }
}