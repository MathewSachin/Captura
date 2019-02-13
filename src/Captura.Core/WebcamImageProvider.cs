using System.Drawing;
using Captura.Models;
using Screna;

namespace Captura.Webcam
{
    public class WebcamImageProvider : IImageProvider
    {
        readonly IWebCamProvider _webCamProvider;

        public WebcamImageProvider(IWebCamProvider WebCamProvider)
        {
            _webCamProvider = WebCamProvider;
        }

        public void Dispose() { }

        public IEditableFrame Capture()
        {
            try
            {
                var img = _webCamProvider.Capture(GraphicsBitmapLoader.Instance);

                if (img is DrawingImage drawingImage && drawingImage.Image is Bitmap bmp)
                    return new GraphicsEditor(bmp);

                return RepeatFrame.Instance;
            }
            catch
            {
                return RepeatFrame.Instance;
            }
        }

        public int Height => _webCamProvider.Height;

        public int Width => _webCamProvider.Width;
    }
}