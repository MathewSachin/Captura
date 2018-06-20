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

        public void Display(IBitmapFrame Frame)
        {
            _previewWindow.Dispatcher.Invoke(() =>
            {
                if (Frame is RepeatFrame)
                    return;

                _lastFrame?.Dispose();

                _lastFrame = Frame;

                if (!_previewWindow.IsVisible)
                    return;
                
                if (_writeableBitmap == null
                    || _writeableBitmap.PixelWidth != Frame.Width
                    || _writeableBitmap.PixelHeight != Frame.Height)
                {
                    _writeableBitmap = new WriteableBitmap(Frame.Width, Frame.Height, 96, 96, PixelFormats.Bgra32, null);
                        
                    _buffer = new byte[Frame.Width * Frame.Height * 4];

                    Console.WriteLine($"Preview Bitmap Allocated: {_buffer.Length}");

                    _previewWindow.DisplayImage.Source = _writeableBitmap;
                }

                if (_previewWindow.DisplayImage.Source == null)
                    _previewWindow.DisplayImage.Source = _writeableBitmap;
                
                Frame.CopyTo(_buffer, _buffer.Length);

                _writeableBitmap.WritePixels(new Int32Rect(0, 0, Frame.Width, Frame.Height), _buffer, Frame.Width * 4, 0);
            });
        }
        
        IBitmapFrame _lastFrame;

        public void Dispose()
        {
            _previewWindow.Dispatcher.Invoke(() =>
            {
                _lastFrame?.Dispose();
                _lastFrame = null;

                _previewWindow.DisplayImage.Source = null;
            });
        }

        public void Show()
        {
            _previewWindow.Dispatcher.Invoke(() => _previewWindow.Show());
        }
    }
}