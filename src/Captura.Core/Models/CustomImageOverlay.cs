using System.Drawing;

namespace Captura.Models
{
    public class CustomImageOverlay : ImageOverlay<CustomImageOverlaySettings>
    {
        readonly Bitmap _bmp;

        public CustomImageOverlay(CustomImageOverlaySettings ImageOverlaySettings) : base(ImageOverlaySettings, false)
        {
            _bmp = new Bitmap(ImageOverlaySettings.Source);
        }

        public override void Dispose()
        {
            _bmp.Dispose();
        }

        protected override Bitmap GetImage()
        {
            return _bmp;
        }
    }
}