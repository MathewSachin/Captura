using System;
using System.Drawing;
using System.Threading;
using Captura.Webcam;

namespace Captura.Models
{
    class WebcamCapture : IWebcamCapture
    {
        readonly Filter _filter;
        readonly Action _onClick;
        CaptureWebcam _captureWebcam;
        readonly SyncContextManager _syncContext = new SyncContextManager();

        public WebcamCapture(Filter Filter, Action OnClick)
        {
            _filter = Filter;
            _onClick = OnClick;
            _captureWebcam = new CaptureWebcam(Filter, OnClick, IntPtr.Zero);

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

            _syncContext.Run(() => image = _captureWebcam.GetFrame(BitmapLoader));

            return image;
        }

        public int Width => _captureWebcam.Size.Width;
        public int Height => _captureWebcam.Size.Height;

        IntPtr _lastWin;

        public void UpdatePreview(IWindow Window, Rectangle Location)
        {
            if (Window != null && _lastWin != Window.Handle)
            {
                Dispose();

                _captureWebcam = new CaptureWebcam(_filter, _onClick, Window.Handle);

                _captureWebcam.StartPreview();

                _lastWin = Window.Handle;
            }

            _captureWebcam.OnPreviewWindowResize(Location.X, Location.Y, Location.Width, Location.Height);
        }
    }
}