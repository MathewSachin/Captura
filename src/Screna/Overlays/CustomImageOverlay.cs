namespace Captura.Models
{
    public class CustomImageOverlay : ImageOverlay<CustomImageOverlaySettings>
    {
        IBitmapImage _bmp;

        public CustomImageOverlay(CustomImageOverlaySettings ImageOverlaySettings)
            : base(ImageOverlaySettings, false) { }

        public override void Dispose()
        {
            _bmp?.Dispose();
        }

        protected override IBitmapImage GetImage(IEditableFrame Editor)
        {
            return _bmp ?? (_bmp = Editor.LoadBitmap(Settings.Source));
        }
    }
}