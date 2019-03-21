using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using Captura.Models;
using Captura.ViewModels;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using Screna;
using Color = System.Windows.Media.Color;
using Point = System.Windows.Point;

namespace Captura
{
    public partial class OverlayWindow
    {
        OverlayWindow()
        {
            InitializeComponent();

            Loaded += OnLoaded;

            Closing += (S, E) =>
            {
                ServiceProvider.Get<Settings>().Save();
            };

            UpdateBackground();
        }

        static OverlayWindow _instance;

        public static void ShowInstance()
        {
            if (_instance == null)
            {
                _instance = new OverlayWindow();

                _instance.Closed += (S, E) => _instance = null;
            }

            _instance.ShowAndFocus();
        }

        void AddToGrid(LayerFrame Frame, bool CanResize)
        {
            Grid.Children.Add(Frame);

            Panel.SetZIndex(Frame, 0);

            var layer = AdornerLayer.GetAdornerLayer(Frame);
            var adorner = new OverlayPositionAdorner(Frame, CanResize);
            layer.Add(adorner);

            adorner.PositionUpdated += Frame.RaisePositionChanged;
        }

        LayerFrame Generate(PositionedOverlaySettings Settings, string Text, Color BackgroundColor)
        {
            var control = new LayerFrame
            {
                Border =
                {
                    Background = new SolidColorBrush(BackgroundColor)
                },
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                Label =
                {
                    Content = Text,
                    Foreground = new SolidColorBrush(Colors.White)
                }
            };

            void Update()
            {
                int left = 0, top = 0, right = 0, bottom = 0;

                switch (Settings.HorizontalAlignment)
                {
                    case Alignment.Start:
                        control.HorizontalAlignment = HorizontalAlignment.Left;
                        left = Settings.X;
                        break;

                    case Alignment.Center:
                        control.HorizontalAlignment = HorizontalAlignment.Center;
                        left = Settings.X;
                        break;

                    case Alignment.End:
                        control.HorizontalAlignment = HorizontalAlignment.Right;
                        right = Settings.X;
                        break;
                }

                switch (Settings.VerticalAlignment)
                {
                    case Alignment.Start:
                        control.VerticalAlignment = VerticalAlignment.Top;
                        top = Settings.Y;
                        break;

                    case Alignment.Center:
                        control.VerticalAlignment = VerticalAlignment.Center;
                        top = Settings.Y;
                        break;

                    case Alignment.End:
                        control.VerticalAlignment = VerticalAlignment.Bottom;
                        bottom = Settings.Y;
                        break;
                }

                Dispatcher.Invoke(() => control.Margin = new Thickness(left, top, right, bottom));
            }

            Settings.PropertyChanged += (S, E) => Update();

            Update();

            control.PositionUpdated += Rect =>
            {
                Settings.X = (int)Rect.X;
                Settings.Y = (int)Rect.Y;
            };

            return control;
        }

        void Bind(FrameworkElement Control, DependencyProperty DependencyProperty, IReactiveProperty Property)
        {
            Control.SetBinding(DependencyProperty,
                new Binding(nameof(Property.Value))
                {
                    Source = Property,
                    Mode = BindingMode.TwoWay
                });
        }

        void Bind<T>(FrameworkElement Control, DependencyProperty DependencyProperty, ReadOnlyReactivePropertySlim<T> Property)
        {
            Control.SetBinding(DependencyProperty,
                new Binding(nameof(Property.Value))
                {
                    Source = Property,
                    Mode = BindingMode.OneWay
                });
        }

        LayerFrame Image(ImageOverlaySettings Settings, string Text)
        {
            var control = Generate(Settings, Text, Colors.Brown);

            var widthProp = Settings
                .ToReactivePropertyAsSynchronized(M => M.ResizeWidth,
                    M => (double) M, M => (int) M);

            var heightProp = Settings
                .ToReactivePropertyAsSynchronized(M => M.ResizeHeight,
                    M => (double)M, M => (int)M);

            Bind(control, WidthProperty, widthProp);
            Bind(control, HeightProperty, heightProp);

            var opacityProp = Settings
                .ObserveProperty(M => M.Opacity)
                .Select(M => M / 100.0)
                .ToReadOnlyReactivePropertySlim();

            Bind(control, OpacityProperty, opacityProp);

            return control;
        }

        LayerFrame Webcam(WebcamOverlaySettings Settings)
        {
            return Image(Settings, "Webcam");
        }

        LayerFrame Text(TextOverlaySettings Settings, string Text)
        {
            var control = Generate(Settings, Text, ConvertColor(Settings.BackgroundColor));

            var fontFamilyProp = Settings
                .ObserveProperty(M => M.FontFamily)
                .Select(M => new FontFamily(M))
                .ToReadOnlyReactivePropertySlim();

            Bind(control.Label, FontFamilyProperty, fontFamilyProp);

            var fontSizeProp = Settings
                .ObserveProperty(M => M.FontSize)
                .ToReadOnlyReactivePropertySlim();

            Bind(control.Label, FontSizeProperty, fontSizeProp);

            var paddingProp = new[]
                {
                    Settings
                        .ObserveProperty(M => M.HorizontalPadding),
                    Settings
                        .ObserveProperty(M => M.VerticalPadding)
                }
                .CombineLatest(M =>
                {
                    var w = M[0];
                    var h = M[1];

                    return new Thickness(w, h, w, h);
                })
                .ToReadOnlyReactivePropertySlim();

            // Border.PaddingProperty is different from PaddingProperty
            Bind(control.Border, Border.PaddingProperty, paddingProp);

            var foregroundProp = Settings
                .ObserveProperty(M => M.FontColor)
                .Select(M => new SolidColorBrush(ConvertColor(M)))
                .ToReadOnlyReactivePropertySlim();

            Bind(control.Label, ForegroundProperty, foregroundProp);

            var backgroundProp = Settings
                .ObserveProperty(M => M.BackgroundColor)
                .Select(M => new SolidColorBrush(ConvertColor(M)))
                .ToReadOnlyReactivePropertySlim();

            Bind(control.Border, BackgroundProperty, backgroundProp);

            var borderThicknessProp = Settings
                .ObserveProperty(M => M.BorderThickness)
                .Select(M => new Thickness(M))
                .ToReadOnlyReactivePropertySlim();

            Bind(control.Border, BorderThicknessProperty, borderThicknessProp);

            var borderBrushProp = Settings
                .ObserveProperty(M => M.BorderColor)
                .Select(M => new SolidColorBrush(ConvertColor(M)))
                .ToReadOnlyReactivePropertySlim();

            Bind(control.Border, BorderBrushProperty, borderBrushProp);

            var cornerRadiusProp = Settings
                .ObserveProperty(M => M.CornerRadius)
                .Select(M => new CornerRadius(M))
                .ToReadOnlyReactivePropertySlim();

            Bind(control.Border, Border.CornerRadiusProperty, cornerRadiusProp);

            return control;
        }

        LayerFrame Censor(CensorOverlaySettings Settings)
        {
            var control = Generate(Settings, "Censored", Colors.Black);

            var widthProp = Settings
                .ToReactivePropertyAsSynchronized(M => M.Width,
                    M => (double)M, M => (int)M);

            var heightProp = Settings
                .ToReactivePropertyAsSynchronized(M => M.Height,
                    M => (double)M, M => (int)M);

            Bind(control, WidthProperty, widthProp);
            Bind(control, HeightProperty, heightProp);

            return control;
        }

        static Color ConvertColor(System.Drawing.Color C)
        {
            return Color.FromArgb(C.A, C.R, C.G, C.B);
        }

        LayerFrame Keystrokes(KeystrokesSettings Settings)
        {
            var control = Text(Settings, "Keystrokes");

            var visibilityProp = Settings
                .ObserveProperty(M => M.SeparateTextFile)
                .Select(M => M ? Visibility.Collapsed : Visibility.Visible)
                .ToReadOnlyReactivePropertySlim();

            Bind(control, VisibilityProperty, visibilityProp);

            return control;
        }

        readonly List<LayerFrame> _textOverlays = new List<LayerFrame>();
        readonly List<LayerFrame> _imageOverlays = new List<LayerFrame>();
        readonly List<LayerFrame> _censorOverlays = new List<LayerFrame>();

        void UpdateCensorOverlays(IEnumerable<CensorOverlaySettings> Settings)
        {
            foreach (var overlay in _censorOverlays)
            {
                Grid.Children.Remove(overlay);
            }

            _censorOverlays.Clear();

            foreach (var setting in Settings)
            {
                var control = Censor(setting);

                var visibilityProp = setting
                    .ObserveProperty(M => M.Display)
                    .Select(M => M ? Visibility.Visible : Visibility.Collapsed)
                    .ToReadOnlyReactivePropertySlim();

                Bind(control, VisibilityProperty, visibilityProp);

                _censorOverlays.Add(control);
            }

            foreach (var overlay in _censorOverlays)
            {
                AddToGrid(overlay, true);

                Panel.SetZIndex(overlay, -1);
            }
        }

        void UpdateTextOverlays(IEnumerable<CustomOverlaySettings> Settings)
        {
            foreach (var overlay in _textOverlays)
            {
                Grid.Children.Remove(overlay);
            }

            _textOverlays.Clear();

            foreach (var setting in Settings)
            {
                var control = Text(setting, setting.Text);

                var visibilityProp = setting
                    .ObserveProperty(M => M.Display)
                    .Select(M => M ? Visibility.Visible : Visibility.Collapsed)
                    .ToReadOnlyReactivePropertySlim();

                Bind(control, VisibilityProperty, visibilityProp);

                var textProp = setting
                    .ObserveProperty(M => M.Text)
                    .ToReadOnlyReactivePropertySlim();

                Bind(control.Label, ContentProperty, textProp);

                _textOverlays.Add(control);
            }

            foreach (var overlay in _textOverlays)
            {
                AddToGrid(overlay, false);

                Panel.SetZIndex(overlay, 1);
            }
        }

        void UpdateImageOverlays(IEnumerable<CustomImageOverlaySettings> Settings)
        {
            foreach (var overlay in _imageOverlays)
            {
                Grid.Children.Remove(overlay);
            }

            _imageOverlays.Clear();

            foreach (var setting in Settings)
            {
                var control = Image(setting, setting.Source);

                var visibilityProp = setting
                    .ObserveProperty(M => M.Display)
                    .Select(M => M ? Visibility.Visible : Visibility.Collapsed)
                    .ToReadOnlyReactivePropertySlim();

                Bind(control, VisibilityProperty, visibilityProp);

                var srcProp = setting
                    .ObserveProperty(M => M.Source)
                    .ToReadOnlyReactivePropertySlim();

                Bind(control.Label, ContentProperty, srcProp);

                _imageOverlays.Add(control);
            }

            foreach (var overlay in _imageOverlays)
            {
                AddToGrid(overlay, true);

                Panel.SetZIndex(overlay, 2);
            }
        }
        
        void OnLoaded(object Sender, RoutedEventArgs RoutedEventArgs)
        {
            PlaceOverlays();
        }

        async void UpdateBackground()
        {
            var vm = ServiceProvider.Get<VideoSourcesViewModel>();

            IBitmapImage bmp;

            switch (vm.SelectedVideoSourceKind?.Source)
            {
                case NoVideoItem _:
                    bmp = ScreenShot.Capture();
                    break;

                default:
                    var screenShotModel = ServiceProvider.Get<ScreenShotModel>();
                    bmp = await screenShotModel.GetScreenShot(true);
                    break;
            }

            using (bmp)
            {
                var stream = new MemoryStream();
                bmp.Save(stream, ImageFormats.Png);

                stream.Seek(0, SeekOrigin.Begin);

                var decoder = new PngBitmapDecoder(stream, BitmapCreateOptions.None, BitmapCacheOption.Default);
                Img.Source = decoder.Frames[0];
            }
        }

        void UpdateScale()
        {
            if (Img.Source == null)
                return;

            var scaleX = Img.ActualWidth / Img.Source.Width;
            var scaleY = Img.ActualHeight / Img.Source.Height;

            Scale.ScaleX = scaleX / Dpi.X;
            Scale.ScaleY = scaleY / Dpi.Y;
        }

        void PlaceOverlays()
        {
            var settings = ServiceProvider.Get<Settings>();

            var censorOverlayVm = ServiceProvider.Get<CensorOverlaysViewModel>();

            UpdateCensorOverlays(censorOverlayVm.Collection);
            (censorOverlayVm.Collection as INotifyCollectionChanged).CollectionChanged += (S, E) => UpdateCensorOverlays(censorOverlayVm.Collection);

            PrepareMousePointer(settings.MousePointerOverlay);
            PrepareMouseClick(settings.Clicks);

            var webcam = Webcam(settings.WebcamOverlay);
            AddToGrid(webcam, true);

            var keystrokes = Keystrokes(settings.Keystrokes);
            AddToGrid(keystrokes, false);

            var elapsed = Text(settings.Elapsed, "00:00:00");
            AddToGrid(elapsed, false);

            var textOverlayVm = ServiceProvider.Get<CustomOverlaysViewModel>();

            UpdateTextOverlays(textOverlayVm.Collection);
            (textOverlayVm.Collection as INotifyCollectionChanged).CollectionChanged += (S, E) => UpdateTextOverlays(textOverlayVm.Collection);

            var imgOverlayVm = ServiceProvider.Get<CustomImageOverlaysViewModel>();

            UpdateImageOverlays(imgOverlayVm.Collection);
            (imgOverlayVm.Collection as INotifyCollectionChanged).CollectionChanged += (S, E) => UpdateImageOverlays(imgOverlayVm.Collection);
        }

        void PrepareMouseClick(MouseClickSettings Settings)
        {
            void Update()
            {
                var d = (Settings.Radius + Settings.BorderThickness) * 2;

                MouseClick.Width = MouseClick.Height = d;
                MouseClick.StrokeThickness = Settings.BorderThickness;
                MouseClick.Stroke = new SolidColorBrush(ConvertColor(Settings.BorderColor));
            }

            Update();
            
            Settings.PropertyChanged += (S, E) => Dispatcher.Invoke(Update);
        }

        void PrepareMousePointer(MouseOverlaySettings Settings)
        {
            void Update()
            {
                var d = (Settings.Radius + Settings.BorderThickness) * 2;

                MousePointer.Width = MousePointer.Height = d;
                MousePointer.StrokeThickness = Settings.BorderThickness;
                MousePointer.Stroke = new SolidColorBrush(ConvertColor(Settings.BorderColor));
                MousePointer.Fill = new SolidColorBrush(ConvertColor(Settings.Color));
            }

            Update();

            Settings.PropertyChanged += (S, E) => Dispatcher.Invoke(Update);
        }

        void OverlayWindow_OnSizeChanged(object Sender, SizeChangedEventArgs E)
        {
            UpdateScale();
        }

        void Img_OnLoaded(object Sender, RoutedEventArgs E)
        {
            UpdateScale();
        }

        static Color GetClickColor(MouseButton Button)
        {
            var settings = ServiceProvider.Get<Settings>();

            switch (Button)
            {
                case MouseButton.Middle:
                    return ConvertColor(settings.Clicks.MiddleClickColor);

                case MouseButton.Right:
                    return ConvertColor(settings.Clicks.RightClickColor);
                    
                default:
                    return ConvertColor(settings.Clicks.Color);
            }
        }

        bool _dragging;

        void UpdateMouseClickPosition(Point Position)
        {
            MouseClick.Margin = new Thickness(Position.X - MouseClick.ActualWidth / 2, Position.Y - MouseClick.ActualHeight / 2, 0, 0);
        }

        void UIElement_OnMouseDown(object Sender, MouseButtonEventArgs E)
        {
            _dragging = true;

            UpdateMouseClickPosition(E.GetPosition(Grid));

            MouseClick.Fill = new SolidColorBrush(GetClickColor(E.ChangedButton));

            MouseClick.BeginAnimation(OpacityProperty, new DoubleAnimation(1, new Duration(TimeSpan.FromMilliseconds(200))));
        }

        void MouseClickEnd()
        {
            MouseClick.BeginAnimation(OpacityProperty, new DoubleAnimation(0, new Duration(TimeSpan.FromMilliseconds(300))));

            _dragging = false;
        }

        void UIElement_OnMouseUp(object Sender, MouseButtonEventArgs E)
        {
            MouseClickEnd();
        }

        bool IsOutsideGrid(Point Point)
        {
            return Point.X <= 0 || Point.Y <= 0
                   || Point.X + MouseClick.ActualWidth / 2 >= Grid.ActualWidth
                   || Point.Y + MouseClick.ActualHeight / 2 >= Grid.ActualHeight;
        }

        void UIElement_OnMouseMove(object Sender, MouseEventArgs E)
        {
            if (ServiceProvider.Get<Settings>().MousePointerOverlay.Display)
                MousePointer.Visibility = Visibility.Visible;

            var position = E.GetPosition(Grid);

            if (IsOutsideGrid(position))
            {
                MousePointer.Visibility = Visibility.Collapsed;

                return;
            }

            if (_dragging)
            {
                UpdateMouseClickPosition(position);
            }

            position.X -= MouseClick.ActualWidth / 2;
            position.Y -= MouseClick.ActualHeight / 2;

            MousePointer.Margin = new Thickness(position.X, position.Y, 0, 0);
        }

        void UIElement_OnMouseLeave(object Sender, MouseEventArgs E)
        {
            MouseClickEnd();

            MousePointer.Visibility = Visibility.Collapsed;
        }
    }
}
