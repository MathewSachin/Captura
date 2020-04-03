using System;
using System.Drawing;
using Captura.Video;
using Reactive.Bindings;

namespace Captura.Webcam
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class WebcamOverlay : ImageOverlay
    {
        WebcamModel _webcamModel;
        IReadOnlyReactiveProperty<IWebcamCapture> _webcamCapture;
        readonly WebcamOverlaySettings _settings;

        public WebcamOverlay(WebcamModel WebcamModel, Settings Settings) : base(true)
        {
            _webcamModel = WebcamModel;
            _settings = Settings.WebcamOverlay;

            _webcamCapture = WebcamModel.InitCapture();
        }

        public override void Draw(IEditableFrame Editor, Func<Point, Point> PointTransform = null)
        {
            // No Webcam
            if (_webcamModel.AvailableCams.Count < 1 || _webcamModel.SelectedCam is NoWebcamItem)
                return;

            var cap = _webcamCapture.Value;

            if (cap == null)
                return;

            var frameSize = new Size((int) Editor.Width, (int) Editor.Height);
            var webcamSize = new Size(cap.Width, cap.Height);

            var pos = _settings.GetPosition(frameSize, webcamSize);
            var size = _settings.GetSize(frameSize, webcamSize);

            Draw(Editor, GetImage(Editor), pos, size, _settings.Opacity);
        }

        IBitmapImage GetImage(IEditableFrame Editor)
        {
            return _webcamCapture.Value?.Capture(Editor);
        }

        public override void Dispose()
        {
            base.Dispose();

            _webcamModel?.ReleaseCapture();
            _webcamModel = null;
            _webcamCapture = null;
        }
    }
}