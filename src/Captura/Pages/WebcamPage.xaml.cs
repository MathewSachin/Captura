using System;
using System.Drawing;
using System.Reactive.Linq;
using WSize = System.Windows.Size;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using Captura.ViewModels;
using Captura.Webcam;
using Captura.Windows.Gdi;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
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

            control.BindOne(MarginProperty,
                _reactor.Location.Select(M => new Thickness(M.X, M.Y, 0, 0)).ToReadOnlyReactivePropertySlim());

            control.BindOne(WidthProperty,
                _reactor.Size.Select(M => M.Width).ToReadOnlyReactivePropertySlim());
            control.BindOne(HeightProperty,
                _reactor.Size.Select(M => M.Height).ToReadOnlyReactivePropertySlim());

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

                    if (_webcamCapture.Value is { } capture)
                    {
                        _reactor.WebcamSize.OnNext(new WSize(capture.Width, capture.Height));

                        UpdateWebcamPreview();
                    }
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

                _reactor.FrameSize.OnNext(new WSize(Img.ActualWidth, Img.ActualHeight));
            }

            PreviewGrid.LayoutUpdated += (S, E) => OnRegionChange();

            _webcamModel
                .ObserveProperty(M => M.SelectedCam)
                .Subscribe(M => UpdateWebcamPreview());

            _reactor.Location
                .CombineLatest(_reactor.Size, (M, N) =>
                {
                    UpdateWebcamPreview();
                    return 0;
                })
                .Subscribe();

            UpdateWebcamPreview();
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

            var relativePt = PreviewGrid.TranslatePoint(new System.Windows.Point(0, 0), parentWindow);

            var position = _reactor.Location.Value;
            var size = _reactor.Size.Value;

            var rect = new RectangleF((float)(relativePt.X + position.X),
                (float)(relativePt.Y + position.Y),
                (float)(size.Width),
                (float)(size.Height));

            return rect.ApplyDpi();
        }

        void UpdateWebcamPreview()
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
