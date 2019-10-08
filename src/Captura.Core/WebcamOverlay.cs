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
            // No Webcam
            if (_webcamModel.AvailableCams.Count < 1 || _webcamModel.SelectedCam is NoWebcamItem)
                return;

            var cap = _webcamModel.WebcamCapture;

            if (cap == null)
                return;

            var pos = _settings.GetPosition(Editor.Width, Editor.Height);
            var w = _settings.GetWidth(Editor.Width);
            var h = _settings.GetHeight(Editor.Height);

            var imgWbyH = cap.Width / cap.Height;

            var frameWbyH = w / h;

            if (imgWbyH > frameWbyH)
            {
                var newH = w / imgWbyH;

                pos.Y += (h - newH) / 2;
                h = newH;
            }
            else
            {
                var newW = h * imgWbyH;

                pos.X += (w - newW) / 2;
                w = newW;
            }

            Draw(Editor, GetImage(Editor), pos, new SizeF(w, h), _settings.Opacity);
        }

        IBitmapImage GetImage(IEditableFrame Editor)
        {
            return _webcamModel.WebcamCapture?.Capture(Editor);
        }
    }
}