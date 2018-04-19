using System.Drawing;

namespace Captura.Models
{
    public class WebcamOverlay : ImageOverlay<WebcamOverlaySettings>
    {
        readonly IWebCamProvider _webCamProvider;

        public WebcamOverlay(IWebCamProvider WebCamProvider, Settings Settings) : base(Settings.WebcamOverlay, true)
        {
            _webCamProvider = WebCamProvider;
        }

        public override Bitmap GetImage()
        {
            // No Webcam
            if (_webCamProvider.AvailableCams.Count < 1 || _webCamProvider.SelectedCam == _webCamProvider.AvailableCams[0])
                return null;

            return _webCamProvider.Capture();
        }
    }
}