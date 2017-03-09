using Screna;
using System.Drawing;
using System.Windows;

namespace Captura
{
    class StaticRegionProvider : IImageProvider
    {
        public StaticRegionProvider()
        {
            var regSel = RegionSelector.Instance;

            var rect = regSel.Rectangle;
            Height = rect.Height;
            Width = rect.Width;

            // Prevent Resize
            regSel.ResizeMode = ResizeMode.NoResize;
        }
        
        public Bitmap Capture() => ScreenShot.Capture(RegionSelector.Instance.Rectangle);

        public int Height { get; }

        public int Width { get; }

        // Make resizable again after capture, invoke on UI Thread
        public void Dispose() => RegionSelector.Instance.Dispatcher.Invoke(() => RegionSelector.Instance.ResizeMode = ResizeMode.CanResize);
    }
}
