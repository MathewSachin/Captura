using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Captura.Models
{
    public class PreviewWindowService : IPreviewWindow
    {
        readonly PreviewWindow _previewWindow = new PreviewWindow();

        bool _visible;

        public PreviewWindowService()
        {
            // Prevent Closing by User
            _previewWindow.Closing += (S, E) =>
            {
                E.Cancel = true;

                _previewWindow.Hide();
            };

            _previewWindow.IsVisibleChanged += (S, E) => _visible = _previewWindow.IsVisible;
        }

        WriteableBitmap _writeableBitmap;
        byte[] _buffer;

        DateTime _timestamp;
        readonly TimeSpan _minInterval = TimeSpan.FromMilliseconds(200);

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

            if (!_visible || DateTime.Now - _timestamp < _minInterval)
            {
                Frame.Dispose();

                return;
            }

            _timestamp = DateTime.Now;

            using (Frame)
                Frame.CopyTo(_buffer, _buffer.Length);

            _previewWindow.Dispatcher.Invoke(() =>
            {
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