using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Captura.Video;

namespace Captura.ViewModels
{
    public class ScreenPickerViewModel
    {
        public ScreenPickerViewModel(IScreen Screen, double Scale)
        {
            this.Screen = Screen;

            using var bmp = ScreenShot.Capture(Screen.Rectangle);
            var stream = new MemoryStream();
            bmp.Save(stream, ImageFormats.Png);

            stream.Seek(0, SeekOrigin.Begin);

            var decoder = new PngBitmapDecoder(stream, BitmapCreateOptions.None, BitmapCacheOption.Default);
            Image = new ImageBrush(decoder.Frames[0]);

            Left = (Screen.Rectangle.Left / Dpi.X - SystemParameters.VirtualScreenLeft) * Scale;
            Top = (Screen.Rectangle.Top / Dpi.Y - SystemParameters.VirtualScreenTop) * Scale;

            Width = Screen.Rectangle.Width / Dpi.X * Scale;
            Height = Screen.Rectangle.Height / Dpi.Y * Scale;
        }

        public double Left { get; }
        public double Top { get; }

        public double Width { get; }
        public double Height { get; }

        public IScreen Screen { get; }

        public Brush Image { get; }
    }
}
