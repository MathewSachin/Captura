using System;
using System.Drawing;
using Captura.Windows.Gdi;
using Reactive.Bindings;

namespace Captura.Webcam
{
    public class WebcamImageProvider : IImageProvider
    {
        WebcamModel _webcamModel;
        IReadOnlyReactiveProperty<IWebcamCapture> _webcamCapture;

        public WebcamImageProvider(WebcamModel WebcamModel)
        {
            _webcamModel = WebcamModel;
            _webcamCapture = WebcamModel.InitCapture();
        }

        public void Dispose()
        {
            _webcamModel?.ReleaseCapture();
            _webcamModel = null;
            _webcamCapture = null;
        }

        public IEditableFrame Capture()
        {
            try
            {
                var img = _webcamCapture.Value?.Capture(GraphicsBitmapLoader.Instance);

                if (img is DrawingImage drawingImage && drawingImage.Image is Bitmap bmp)
                    return new GraphicsEditor(bmp);

                return RepeatFrame.Instance;
            }
            catch
            {
                return RepeatFrame.Instance;
            }
        }

        public IBitmapFrame DummyFrame => DrawingFrame.DummyFrame;

        public int Height => _webcamCapture.Value?.Height ?? 0;

        public int Width => _webcamCapture.Value?.Width ?? 0;

        public Func<Point, Point> PointTransform { get; } = P => P;
    }
}