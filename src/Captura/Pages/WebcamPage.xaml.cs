using System;
using System.Drawing;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using Captura.ViewModels;
using Reactive.Bindings.Extensions;
using Screna;
using Xceed.Wpf.Toolkit.Core.Utilities;

namespace Captura
{
    public partial class WebcamPage
    {
        readonly WebcamModel _webcamModel;
        readonly ScreenShotModel _screenShotModel;
        readonly IPlatformServices _platformServices;

        public WebcamPage(WebcamModel WebcamModel,
            ScreenShotModel ScreenShotModel,
            IPlatformServices PlatformServices)
        {
            _webcamModel = WebcamModel;
            _screenShotModel = ScreenShotModel;
            _platformServices = PlatformServices;

            InitializeComponent();
        }

        public void SetupPreview()
        {
            // Open Preview Window
            //_webcamModel.PreviewClicked += this.ShowAndFocus;

            IsVisibleChanged += (S, E) => SwitchWebcamPreview();

            void OnSizeChange()
            {
                var rect = GetPreviewWindowRect();

                _webcamModel.WebcamCapture?.UpdatePreview(null, rect);
            }

            PreviewTarget.SizeChanged += (S, E) => OnSizeChange();

            _webcamModel
                .ObserveProperty(M => M.SelectedCam)
                .Where(M => _webcamModel.WebcamCapture != null)
                .Subscribe(M => SwitchWebcamPreview());

            SwitchWebcamPreview();
        }

        async void CaptureImage_OnClick(object Sender, RoutedEventArgs E)
        {
            try
            {
                var img = _webcamModel.WebcamCapture?.Capture(GraphicsBitmapLoader.Instance);

                await _screenShotModel.SaveScreenShot(img);
            }
            catch { }
        }

        Rectangle GetPreviewWindowRect()
        {
            var parentWindow = VisualTreeHelperEx.FindAncestorByType<Window>(this);

            var relativePt = PreviewTarget.TranslatePoint(new System.Windows.Point(0, 0), parentWindow);

            var rect = new RectangleF((float) relativePt.X, (float) relativePt.Y, (float) PreviewTarget.ActualWidth, (float) PreviewTarget.ActualHeight);

            return rect.ApplyDpi();
        }

        void SwitchWebcamPreview()
        {
            if (_webcamModel.WebcamCapture == null)
                return;

            if (IsVisible)
            {
                if (PresentationSource.FromVisual(this) is HwndSource source)
                {
                    var win = _platformServices.GetWindow(source.Handle);

                    var rect = GetPreviewWindowRect();

                    _webcamModel.WebcamCapture.UpdatePreview(win, rect);
                }
            }
            else if (PresentationSource.FromVisual(MainWindow.Instance) is HwndSource source)
            {
                var win = _platformServices.GetWindow(source.Handle);

                var rect = new RectangleF(280, 1, 50, 40).ApplyDpi();

                _webcamModel.WebcamCapture.UpdatePreview(win, rect);
            }
        }
    }
}
