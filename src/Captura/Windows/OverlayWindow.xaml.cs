using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using Captura.Models;
using Captura.ViewModels;
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

        LayerFrame Image(ImageOverlaySettings Settings, string Text)
        {
            var control = Generate(Settings, Text, Colors.Brown);

            control.Width = Settings.ResizeWidth;
            control.Height = Settings.ResizeHeight;

            control.Opacity = Settings.Opacity / 100.0;

            Settings.PropertyChanged += (S, E) =>
            {
                Dispatcher.Invoke(() =>
                {
                    control.Width = Settings.ResizeWidth;
                    control.Height = Settings.ResizeHeight;
                    control.Opacity = Settings.Opacity / 100.0;
                });
            };

            control.PositionUpdated += Rect =>
            {
                Settings.ResizeWidth = (int)Rect.Width;
                Settings.ResizeHeight = (int)Rect.Height;
            };

            return control;
        }

        LayerFrame Webcam(WebcamOverlaySettings Settings)
        {
            return Image(Settings, "Webcam");
        }

        LayerFrame Text(TextOverlaySettings Settings, string Text)
        {
            var control = Generate(Settings, Text, ConvertColor(Settings.BackgroundColor));
            
            control.Label.FontSize = Settings.FontSize;

            control.Border.Padding = new Thickness(Settings.HorizontalPadding,
                Settings.VerticalPadding,
                Settings.HorizontalPadding,
                Settings.VerticalPadding);

            control.Label.Foreground = new SolidColorBrush(ConvertColor(Settings.FontColor));
            control.Border.BorderThickness = new Thickness(Settings.BorderThickness);
            control.Border.BorderBrush = new SolidColorBrush(ConvertColor(Settings.BorderColor));

            control.Border.CornerRadius = new CornerRadius(Settings.CornerRadius);

            Settings.PropertyChanged += (S, E) =>
            {
                switch (E.PropertyName)
                {
                    case nameof(Settings.BackgroundColor):
                        control.Border.Background = new SolidColorBrush(ConvertColor(Settings.BackgroundColor));
                        break;

                    case nameof(Settings.FontColor):
                        control.Label.Foreground = new SolidColorBrush(ConvertColor(Settings.FontColor));
                        break;

                    case nameof(Settings.BorderThickness):
                        control.Border.BorderThickness = new Thickness(Settings.BorderThickness);
                        break;

                    case nameof(Settings.BorderColor):
                        control.Border.BorderBrush = new SolidColorBrush(ConvertColor(Settings.BorderColor));
                        break;

                    case nameof(Settings.FontSize):
                        control.Label.FontSize = Settings.FontSize;
                        break;

                    case nameof(Settings.HorizontalPadding):
                    case nameof(Settings.VerticalPadding):
                        control.Border.Padding = new Thickness(Settings.HorizontalPadding,
                            Settings.VerticalPadding,
                            Settings.HorizontalPadding,
                            Settings.VerticalPadding);
                        break;

                    case nameof(Settings.CornerRadius):
                        control.Border.CornerRadius = new CornerRadius(Settings.CornerRadius);
                        break;
                }
            };

            return control;
        }

        LayerFrame Censor(CensorOverlaySettings Settings)
        {
            var control = Generate(Settings, "Censored", Colors.Black);

            control.Width = Settings.Width;
            control.Height = Settings.Height;

            Settings.PropertyChanged += (S, E) =>
            {
                Dispatcher.Invoke(() =>
                {
                    control.Width = Settings.Width;
                    control.Height = Settings.Height;
                });
            };

            control.PositionUpdated += Rect =>
            {
                Settings.Width = (int)Rect.Width;
                Settings.Height = (int)Rect.Height;
            };

            return control;
        }

        static Color ConvertColor(System.Drawing.Color C)
        {
            return Color.FromArgb(C.A, C.R, C.G, C.B);
        }

        LayerFrame Keystrokes(KeystrokesSettings Settings)
        {
            var control = Text(Settings, "Keystrokes");

            void SetVisibility()
            {
                control.Visibility = Settings.SeparateTextFile ? Visibility.Collapsed : Visibility;
            }

            SetVisibility();

            Settings.PropertyChanged += (S, E) =>
            {
                switch (E.PropertyName)
                {
                    case nameof(Settings.SeparateTextFile):
                        SetVisibility();
                        break;
                }
            };

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
                control.Visibility = setting.Display ? Visibility.Visible : Visibility.Collapsed;

                setting.PropertyChanged += (S, E) =>
                {
                    switch (E.PropertyName)
                    {
                        case nameof(setting.Display):
                            control.Visibility = setting.Display ? Visibility.Visible : Visibility.Collapsed;
                            break;
                    }
                };

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
                control.Visibility = setting.Display ? Visibility.Visible : Visibility.Collapsed;

                setting.PropertyChanged += (S, E) =>
                {
                    switch (E.PropertyName)
                    {
                        case nameof(setting.Text):
                            control.Label.Content = setting.Text;
                            break;

                        case nameof(setting.Display):
                            control.Visibility = setting.Display ? Visibility.Visible : Visibility.Collapsed;
                            break;
                    }
                };

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
                control.Visibility = setting.Display ? Visibility.Visible : Visibility.Collapsed;

                setting.PropertyChanged += (S, E) =>
                {
                    switch (E.PropertyName)
                    {
                        case nameof(setting.Source):
                            control.Label.Content = setting.Source;
                            break;

                        case nameof(setting.Display):
                            control.Visibility = setting.Display ? Visibility.Visible : Visibility.Collapsed;
                            break;
                    }
                };

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
            var vm = ServiceProvider.Get<MainViewModel>();

            Bitmap bmp;

            switch (vm.VideoViewModel.SelectedVideoSourceKind?.Source)
            {
                case FullScreenItem _:
                case NoVideoItem _:
                    bmp = ScreenShot.Capture();
                    break;

                default:
                    bmp = await vm.ScreenShotViewModel.GetScreenShot();
                    break;
            }

            using (bmp)
            {
                var stream = new MemoryStream();
                bmp.Save(stream, ImageFormat.Png);

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
