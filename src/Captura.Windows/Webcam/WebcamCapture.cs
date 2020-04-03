using System;
using System.Drawing;
using Captura.Models;
using Captura.Video;

namespace Captura.Webcam
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
            _syncContext.Run(() =>
            {
                _captureWebcam.StopPreview();
                _captureWebcam.Dispose();
            });
        }

        public IBitmapImage Capture(IBitmapLoader BitmapLoader)
        {
            return _syncContext.Run(() => _captureWebcam.GetFrame(BitmapLoader));
        }

        public int Width => _captureWebcam.Size.Width;
        public int Height => _captureWebcam.Size.Height;

        IntPtr _lastWin;

        public void UpdatePreview(IWindow Window, Rectangle Location)
        {
            _syncContext.Run(() =>
            {
                if (Window != null && _lastWin != Window.Handle)
                {
                    Dispose();

                    _captureWebcam = new CaptureWebcam(_filter, _onClick, Window.Handle);

                    _captureWebcam.StartPreview();

                    _lastWin = Window.Handle;
                }

                _captureWebcam.OnPreviewWindowResize(Location.X, Location.Y, Location.Width, Location.Height);
            });
        }
    }
}