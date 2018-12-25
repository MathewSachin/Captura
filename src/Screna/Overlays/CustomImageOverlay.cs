using System;
using System.Drawing;

namespace Captura.Models
{
    public class CustomImageOverlay : ImageOverlay<CustomImageOverlaySettings>
    {
        IDisposable _bmp;
        Size _size;

        public CustomImageOverlay(CustomImageOverlaySettings ImageOverlaySettings)
            : base(ImageOverlaySettings, false) { }

        public override void Dispose()
        {
            _bmp?.Dispose();
        }

        protected override IDisposable GetImage(IEditableFrame Editor, out Size Size)
        {
            if (_bmp == null)
            {
                _bmp = Editor.LoadBitmap(Settings.Source, out _size);
            }

            Size = _size;

            return _bmp;
        }
    }
}