using System;
using System.Drawing;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Interop;
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
        readonly WebcamOverlayReactor _reactor;

        public WebcamPage(WebcamModel WebcamModel,
            ScreenShotModel ScreenShotModel,
            IPlatformServices PlatformServices,
            WebcamOverlaySettings WebcamSettings)
        {
            _webcamModel = WebcamModel;
            _screenShotModel = ScreenShotModel;
            _platformServices = PlatformServices;

            _reactor = new WebcamOverlayReactor(WebcamSettings);

            Loaded += OnLoaded;

            InitializeComponent();
        }

        bool _loaded;

        void OnLoaded(object Sender, RoutedEventArgs E)
        {
            if (_loaded)
                return;

            _loaded = true;

            var control = PreviewTarget;

            control.BindOne(MarginProperty, _reactor.Margin);

            control.BindOne(WidthProperty, _reactor.Width);
            control.BindOne(HeightProperty, _reactor.Height);

            control.BindOne(OpacityProperty, _reactor.Opacity);
        }

        public void SetupPreview()
        {
            _webcamModel.PreviewClicked += SettingsWindow.ShowWebcamPage;

            IsVisibleChanged += (S, E) => SwitchWebcamPreview();

            void OnRegionChange()
            {
                if (!IsVisible)
                    return;

                _reactor.FrameSize.OnNext(new System.Windows.Size(PreviewGrid.ActualWidth, PreviewGrid.ActualHeight));

                var rect = GetPreviewWindowRect();

                _webcamModel.WebcamCapture?.UpdatePreview(null, rect);
            }

            PreviewTarget.LayoutUpdated += (S, E) => OnRegionChange();

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

            // Maintain Aspect Ratio
            if (_webcamModel.WebcamCapture != null)
            {
                float w = _webcamModel.WebcamCapture.Width;
                float h = _webcamModel.WebcamCapture.Height;
                var imgWbyH = w / h;

                var frameWbyH = rect.Width / rect.Height;

                if (imgWbyH > frameWbyH)
                {
                    var newH = rect.Width / imgWbyH;

                    rect.Y += (rect.Height - newH) / 2;
                    rect.Height = newH;
                }
                else
                {
                    var newW = rect.Height * imgWbyH;

                    rect.X += (rect.Width - newW) / 2;
                    rect.Width = newW;
                }
            }

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
