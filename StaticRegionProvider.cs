using Screna;
using System.Drawing;
using System.Windows;
using Point = System.Drawing.Point;

namespace Captura
{
    class StaticRegionProvider : IImageProvider
    {
        RegionSelector RegSel;
        IOverlay[] Overlays;

        public StaticRegionProvider(RegionSelector RegSel, params IOverlay[] Overlays)
        {
            this.RegSel = RegSel;

            Height = (int)RegSel.Height;
            Width = (int)RegSel.Width;

            RegSel.ResizeMode = ResizeMode.NoResize;

            this.Overlays = Overlays;
        }

        Rectangle Rectangle
        {
            get
            {
                var Location = RegSel.Dispatcher.Invoke(() => new Point((int)RegSel.Left, (int)RegSel.Top));
                return new Rectangle(Location.X, Location.Y, Width, Height);
            }
        }

        public Bitmap Capture()
        {
            var BMP = ScreenShot.Capture(Rectangle);

            using (var g = Graphics.FromImage(BMP))
                foreach (var overlay in Overlays)
                    overlay.Draw(g, Rectangle.Location);

            return BMP;
        }

        public int Height { get; }

        public int Width { get; }
        
        public void Dispose() { RegSel.ResizeMode = ResizeMode.CanResize; }
    }
}
