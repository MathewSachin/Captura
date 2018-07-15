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

        public IBitmapFrame Capture()
        {
            try
            {
                var img = _webCamProvider.Capture();

                if (img == null)
                    return RepeatFrame.Instance;

                return new OneTimeFrame(img);
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