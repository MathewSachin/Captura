using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Screna;

namespace Captura.Models
{
    public class PreviewWindowService : IPreviewWindow
    {
        readonly PreviewWindow _previewWindow = new PreviewWindow();

        public PreviewWindowService()
        {
            // Prevent Closing by User
            _previewWindow.Closing += (S, E) =>
            {
                E.Cancel = true;

                _previewWindow.Hide();
            };
        }

        WriteableBitmap _writeableBitmap;
        byte[] _buffer;

        public void Init(int Width, int Height)
        {
            _previewWindow.Dispatcher.Invoke(() =>
            {
                if (_writeableBitmap != null
                    && _writeableBitmap.PixelWidth == Width
                    && _writeableBitmap.PixelHeight == Height)
                    return;

                _writeableBitmap = new WriteableBitmap(Width, Height, 96, 96, PixelFormats.Bgra32, null);

                _buffer = new byte[Width * Height * 4];

                Console.WriteLine($"Preview Bitmap Allocated: {_buffer.Length}");

                _previewWindow.DisplayImage.Source = _writeableBitmap;
            });
        }

        public void Display(IBitmapFrame Frame)
        {
            if (Frame is RepeatFrame)
                return;

            _previewWindow.Dispatcher.Invoke(() =>
            {
                if (!_previewWindow.IsVisible)
                {
                    Frame.Dispose();

                    return;
                }

                using (Frame)
                    Frame.CopyTo(_buffer, _buffer.Length);

                _writeableBitmap.WritePixels(new Int32Rect(0, 0, Frame.Width, Frame.Height), _buffer, Frame.Width * 4, 0);
            });
        }

        public void Dispose() { }

        public void Show()
        {
            _previewWindow.Dispatcher.Invoke(() => _previewWindow.Show());
        }
    }
}