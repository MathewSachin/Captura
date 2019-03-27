using Captura.ViewModels;

namespace Captura.Models
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class WebcamOverlay : ImageOverlay<WebcamOverlaySettings>
    {
        readonly WebcamModel _webcamModel;

        public WebcamOverlay(WebcamModel WebcamModel, Settings Settings) : base(Settings.WebcamOverlay, true)
        {
            _webcamModel = WebcamModel;
        }

        protected override IBitmapImage GetImage(IEditableFrame Editor)
        {
            // No Webcam
            if (_webcamModel.AvailableCams.Count < 1 || _webcamModel.SelectedCam is NoWebcamItem)
                return null;

            return _webcamModel.WebcamCapture?.Capture(Editor);
        }
    }
}