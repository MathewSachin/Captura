using System;
using Screna;

namespace Captura.Models
{
    public class PreviewWriter : IVideoFileWriter
    {
        readonly PreviewWindow _previewWindow;

        public PreviewWriter()
        {
            _previewWindow = new PreviewWindow();

            _previewWindow.Dispatcher.Invoke(() => _previewWindow.Show());
        }

        public void Dispose()
        {
            _previewWindow.Dispatcher.Invoke(() => _previewWindow.Close());
        }

        IBitmapFrame _lastFrame;

        public void WriteFrame(IBitmapFrame Image)
        {
            if (Image is RepeatFrame)
                return;

            _lastFrame?.Dispose();

            _lastFrame = Image;

            if (!_previewWindow.IsVisible)
            {
                throw new OperationCanceledException();
            }

            _previewWindow.Dispatcher.Invoke(() =>
            {
                _previewWindow.DisplayImage.Image = Image.Bitmap;
            });
        }

        public bool SupportsAudio { get; } = false;

        public void WriteAudio(byte[] Buffer, int Length) { }
    }
}