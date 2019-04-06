using System;
using System.Drawing;
using Captura.ViewModels;
using Screna;

namespace Captura.Webcam
{
    public class WebcamImageProvider : IImageProvider
    {
        readonly WebcamModel _webcamModel;

        public WebcamImageProvider(WebcamModel WebcamModel)
        {
            _webcamModel = WebcamModel;
        }

        public void Dispose() { }

        public IEditableFrame Capture()
        {
            try
            {
                var img = _webcamModel.WebcamCapture?.Capture(GraphicsBitmapLoader.Instance);

                if (img is DrawingImage drawingImage && drawingImage.Image is Bitmap bmp)
                    return new GraphicsEditor(bmp);

                return RepeatFrame.Instance;
            }
            catch
            {
                return RepeatFrame.Instance;
            }
        }

        public int Height => _webcamModel.WebcamCapture?.Height ?? 0;

        public int Width => _webcamModel.WebcamCapture?.Width ?? 0;

        public Func<Point, Point> PointTransform { get; } = P => P;
    }
}