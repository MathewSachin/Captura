using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using Captura.MouseKeyHook;
using Captura.Video;
using Captura.ViewModels;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using Color = System.Windows.Media.Color;
using Point = System.Windows.Point;

namespace Captura
{
    public partial class OverlayPage
    {
        OverlayPage()
        {
            InitializeComponent();

            Loaded += OnLoaded;

            Unloaded += (S, E) => Grid.Children.Clear();
        }

        static readonly Lazy<OverlayPage> LazyInstance = new Lazy<OverlayPage>(() => new OverlayPage());

        public static OverlayPage Instance => LazyInstance.Value;

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

            var vm = new PositionOverlayReactor(Settings);

            control.BindOne(HorizontalAlignmentProperty, vm.HAlignment);
            control.BindOne(VerticalAlignmentProperty, vm.VAlignment);
            control.BindOne(MarginProperty, vm.Margin);

            control.PositionUpdated += Rect =>
            {
                Settings.X = (int)Rect.X;
                Settings.Y = (int)Rect.Y;
            };

            return control;
        }

        LayerFrame Image(ImageOverlaySettings Settings, string Text)
        {
            var control = Generate(Settings, Text, Colors.Brown);

            var vm = new ImageOverlayReactor(Settings);

            control.Bind(WidthProperty, vm.Width);
            control.Bind(HeightProperty, vm.Height);

            control.BindOne(OpacityProperty, vm.Opacity);

            return control;
        }

        LayerFrame Text(TextOverlaySettings Settings, string Text)
        {
            var control = Generate(Settings, Text, Settings.BackgroundColor.ToWpfColor());

            var vm = new TextOverlayReactor(Settings);

            control.Label.BindOne(FontFamilyProperty, vm.FontFamily);
            control.Label.BindOne(FontSizeProperty, vm.FontSize);

            // Border.PaddingProperty is different from PaddingProperty
            control.Border.BindOne(Border.PaddingProperty, vm.Padding);

            control.Label.BindOne(ForegroundProperty, vm.Foreground);
            control.Border.BindOne(BackgroundProperty, vm.Background);

            control.Border.BindOne(Border.BorderThicknessProperty, vm.BorderThickness);
            control.Border.BindOne(Border.BorderBrushProperty, vm.BorderBrush);
            control.Border.BindOne(Border.CornerRadiusProperty, vm.CornerRadius);

            return control;
        }

        LayerFrame Censor(CensorOverlaySettings Settings)
        {
            var control = Generate(Settings, "Censored", Colors.Black);

            var vm = new CensorOverlayReactor(Settings);

            control.Bind(WidthProperty, vm.Width);
            control.Bind(HeightProperty, vm.Height);

            control.BindOne(VisibilityProperty, vm.Visible);

            return control;
        }

        LayerFrame Keystrokes(KeystrokesSettings Settings)
        {
            var control = Text(Settings, "Keystrokes");

            var visibilityProp = Settings
                .ObserveProperty(M => M.SeparateTextFile)
                .Select(M => M ? Visibility.Collapsed : Visibility.Visible)
                .ToReadOnlyReactivePropertySlim();

            control.BindOne(VisibilityProperty, visibilityProp);

            return control;
        }

        readonly List<LayerFrame> _textOverlays = new List<LayerFrame>();
        readonly List<LayerFrame> _imageOverlays = new List<LayerFrame>();
        readonly List<LayerFrame> _censorOverlays = new List<LayerFrame>();

        void UpdateOverlays<TSettings>(IEnumerable<TSettings> Settings,
            List<LayerFrame> LayerFrames,
            Func<TSettings, LayerFrame> LayerFrameGenerator,
            bool CanResize,
            int ZIndex)
        {
            foreach (var layerFrame in LayerFrames)
            {
                Grid.Children.Remove(layerFrame);
            }

            LayerFrames.Clear();

            LayerFrames.AddRange(Settings.Select(LayerFrameGenerator));

            foreach (var layerFrame in LayerFrames)
            {
                AddToGrid(layerFrame, CanResize);

                Panel.SetZIndex(layerFrame, ZIndex);
            }
        }

        void UpdateCensorOverlays(IEnumerable<CensorOverlaySettings> Settings)
        {
            UpdateOverlays(Settings, _censorOverlays, Censor, true, -1);
        }

        void UpdateTextOverlays(IEnumerable<CustomOverlaySettings> Settings)
        {
            UpdateOverlays(Settings, _textOverlays, Setting =>
            {
                var control = Text(Setting, Setting.Text);

                var visibilityProp = Setting
                    .ObserveProperty(M => M.Display)
                    .Select(M => M ? Visibility.Visible : Visibility.Collapsed)
                    .ToReadOnlyReactivePropertySlim();

                control.BindOne(VisibilityProperty, visibilityProp);

                var textProp = Setting
                    .ObserveProperty(M => M.Text)
                    .ToReadOnlyReactivePropertySlim();

                control.Label.BindOne(ContentProperty, textProp);

                return control;
            }, false, 1);
        }

        void UpdateImageOverlays(IEnumerable<CustomImageOverlaySettings> Settings)
        {
            UpdateOverlays(Settings, _imageOverlays, Setting =>
            {
                var control = Image(Setting, Setting.Source);

                var img = new Image
                {
                    Stretch = Stretch.Fill
                };

                control.Label.Content = img;

                var visibilityProp = Setting
                    .ObserveProperty(M => M.Display)
                    .Select(M => M ? Visibility.Visible : Visibility.Collapsed)
                    .ToReadOnlyReactivePropertySlim();

                control.BindOne(VisibilityProperty, visibilityProp);

                var srcProp = Setting
                    .ObserveProperty(M => M.Source)
                    .ToReadOnlyReactivePropertySlim();

                img.BindOne(System.Windows.Controls.Image.SourceProperty, srcProp);

                return control;
            }, true, 2);
        }

        async void OnLoaded(object Sender, RoutedEventArgs RoutedEventArgs)
        {
            await UpdateBackground();

            PlaceOverlays();

            UpdateScale();
        }

        async Task UpdateBackground()
        {
            Img.Source = await WpfExtensions.GetBackground();
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
                MouseClick.Stroke = new SolidColorBrush(Settings.BorderColor.ToWpfColor());
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
                MousePointer.Stroke = new SolidColorBrush(Settings.BorderColor.ToWpfColor());
                MousePointer.Fill = new SolidColorBrush(Settings.Color.ToWpfColor());
            }

            Update();

            Settings.PropertyChanged += (S, E) => Dispatcher.Invoke(Update);
        }

        void OverlayWindow_OnSizeChanged(object Sender, SizeChangedEventArgs E)
        {
            UpdateScale();
        }

        static Color GetClickColor(MouseButton Button)
        {
            var settings = ServiceProvider.Get<Settings>();

            switch (Button)
            {
                case MouseButton.Middle:
                    return settings.Clicks.MiddleClickColor.ToWpfColor();

                case MouseButton.Right:
                    return settings.Clicks.RightClickColor.ToWpfColor();
                    
                default:
                    return settings.Clicks.Color.ToWpfColor();
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
