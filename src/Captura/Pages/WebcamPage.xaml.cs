using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using Captura.Models;
using Captura.ViewModels;
using Reactive.Bindings;
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

        async void OnLoaded(object Sender, RoutedEventArgs E)
        {
            await UpdateBackground();

            if (_loaded)
                return;

            _loaded = true;

            var control = PreviewTarget;

            control.BindOne(MarginProperty, _reactor.Margin);

            control.BindOne(WidthProperty, _reactor.Width);
            control.BindOne(HeightProperty, _reactor.Height);

            control.BindOne(OpacityProperty, _reactor.Opacity);
        }

        async Task UpdateBackground()
        {
            Img.Source = await WpfExtensions.GetBackground();
        }

        IReadOnlyReactiveProperty<IWebcamCapture> _webcamCapture;

        public void SetupPreview()
        {
            _webcamModel.PreviewClicked += SettingsWindow.ShowWebcamPage;

            IsVisibleChanged += (S, E) =>
            {
                if (IsVisible && _webcamCapture == null)
                {
                    _webcamCapture = _webcamModel.InitCapture();

                    SwitchWebcamPreview();
                }
                else if (!IsVisible && _webcamCapture != null)
                {
                    _webcamModel.ReleaseCapture();
                    _webcamCapture = null;
                }
            };

            void OnRegionChange()
            {
                if (!IsVisible)
                    return;

                _reactor.FrameSize.OnNext(new System.Windows.Size(PreviewGrid.ActualWidth, PreviewGrid.ActualHeight));

                SwitchWebcamPreview();
            }

            PreviewTarget.LayoutUpdated += (S, E) => OnRegionChange();

            _webcamModel
                .ObserveProperty(M => M.SelectedCam)
                .Subscribe(M => SwitchWebcamPreview());

            SwitchWebcamPreview();
        }

        async void CaptureImage_OnClick(object Sender, RoutedEventArgs E)
        {
            try
            {
                var img = _webcamCapture.Value?.Capture(GraphicsBitmapLoader.Instance);

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
            if (_webcamCapture?.Value is { } webcamCapture)
            {
                float w = webcamCapture.Width;
                float h = webcamCapture.Height;
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
            if (PresentationSource.FromVisual(this) is HwndSource source)
            {
                var win = _platformServices.GetWindow(source.Handle);

                var rect = GetPreviewWindowRect();

                _webcamCapture?.Value?.UpdatePreview(win, rect);
            }
        }
    }
}
