using System;
using System.Drawing;
using System.Threading;
using Captura.Webcam;

namespace Captura.Models
{
    public class WebcamCapture : IWebcamCapture
    {
        readonly Filter _filter;
        CaptureWebcam _captureWebcam;
        readonly SynchronizationContext _syncContext = SynchronizationContext.Current;

        public WebcamCapture(Filter Filter)
        {
            _filter = Filter;
            _captureWebcam = new CaptureWebcam(Filter, null, IntPtr.Zero);

            _captureWebcam.StartPreview();
        }

        public void Dispose()
        {
            _captureWebcam.StopPreview();
            _captureWebcam.Dispose();
        }

        public IBitmapImage Capture(IBitmapLoader BitmapLoader)
        {
            if (_syncContext == null)
            {
                return _captureWebcam.GetFrame(BitmapLoader);
            }

            IBitmapImage image = null;

            _syncContext.Send(S => image = _captureWebcam.GetFrame(BitmapLoader), null);

            return image;
        }

        public int Width => _captureWebcam.Size.Width;
        public int Height => _captureWebcam.Size.Height;

        public void UpdatePreview(IWindow Window, Rectangle Location)
        {
            if (Window != null)
            {
                Dispose();

                _captureWebcam = new CaptureWebcam(_filter, null, Window.Handle);

                _captureWebcam.StartPreview();
            }

            _captureWebcam.OnPreviewWindowResize(Location.X, Location.Y, Location.Width, Location.Height);
        }
    }
}