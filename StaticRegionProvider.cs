using Screna;
using System.Drawing;
using System.Windows;
using Point = System.Drawing.Point;

namespace Captura
{
    class StaticRegionProvider : IImageProvider
    {
        readonly RegionSelector _regSel;
        readonly IOverlay[] _overlays;

        public StaticRegionProvider(RegionSelector RegSel, params IOverlay[] Overlays)
        {
            _regSel = RegSel;

            Height = (int) RegSel.Height;
            Width = (int) RegSel.Width;

            RegSel.ResizeMode = ResizeMode.NoResize;

            _overlays = Overlays;
        }

        Rectangle Rectangle
        {
            get
            {
                var location = _regSel.Dispatcher.Invoke(() => new Point((int) _regSel.Left, (int) _regSel.Top));
                return new Rectangle(location.X, location.Y, Width, Height);
            }
        }

        public Bitmap Capture()
        {
            var bmp = ScreenShot.Capture(Rectangle);

            using (var g = Graphics.FromImage(bmp))
                foreach (var overlay in _overlays)
                    overlay.Draw(g, Rectangle.Location);

            return bmp;
        }

        public int Height { get; }

        public int Width { get; }

        public void Dispose() => _regSel.ResizeMode = ResizeMode.CanResize;
    }
}
