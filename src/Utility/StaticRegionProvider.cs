using Screna;
using System.Drawing;
using System.Windows;

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
        
        public Bitmap Capture()
        {
            var rect = _regSel.Rectangle;

            var bmp = ScreenShot.Capture(rect);

            using (var g = Graphics.FromImage(bmp))
                foreach (var overlay in _overlays)
                    overlay.Draw(g, rect.Location);

            return bmp;
        }

        public int Height { get; }

        public int Width { get; }

        public void Dispose() => _regSel.ResizeMode = ResizeMode.CanResize;
    }
}
