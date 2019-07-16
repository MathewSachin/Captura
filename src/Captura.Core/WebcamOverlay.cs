using System;
using System.Drawing;
using Captura.ViewModels;

namespace Captura.Models
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class WebcamOverlay : ImageOverlay
    {
        readonly WebcamModel _webcamModel;
        readonly WebcamOverlaySettings _settings;

        public WebcamOverlay(WebcamModel WebcamModel, Settings Settings) : base(true)
        {
            _webcamModel = WebcamModel;
            _settings = Settings.WebcamOverlay;
        }

        public override void Draw(IEditableFrame Editor, Func<Point, Point> PointTransform = null)
        {
            var pos = _settings.GetPosition(Editor.Width, Editor.Height);
            var w = _settings.GetWidth(Editor.Width);
            var h = _settings.GetHeight(Editor.Height);

            Draw(Editor, GetImage(Editor), pos, new SizeF(w, h), _settings.Opacity);
        }

        IBitmapImage GetImage(IEditableFrame Editor)
        {
            // No Webcam
            if (_webcamModel.AvailableCams.Count < 1 || _webcamModel.SelectedCam is NoWebcamItem)
                return null;

            return _webcamModel.WebcamCapture?.Capture(Editor);
        }
    }
}