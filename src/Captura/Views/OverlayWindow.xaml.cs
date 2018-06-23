using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using Captura.Models;
using Captura.ViewModels;
using Screna;
using Color = System.Windows.Media.Color;

namespace Captura
{
    public partial class OverlayWindow
    {
        public OverlayWindow()
        {
            InitializeComponent();

            Loaded += OnLoaded;

            Closing += (S, E) =>
            {
                ServiceProvider.Get<Settings>().Save();
            };
        }

        void AddToGrid(LayerFrame Frame, bool CanResize)
        {
            Grid.Children.Add(Frame);

            var layer = AdornerLayer.GetAdornerLayer(Frame);
            var adorner = new OverlayPositionAdorner(Frame, CanResize);
            layer.Add(adorner);

            adorner.PositionUpdated += Frame.RaisePositionChanged;
        }

        LayerFrame Generate(PositionedOverlaySettings Settings, string Name, Color Background)
        {
            var control = new LayerFrame
            {
                Tag = Name,
                Background = new SolidColorBrush(Background),
                Foreground = new SolidColorBrush(Colors.White),
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top
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

        LayerFrame Image(ImageOverlaySettings Settings, string Name)
        {
            var control = Generate(Settings, Name, Colors.Brown);

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
            
            control.FontSize = Settings.FontSize;

            control.Padding = new Thickness(Settings.HorizontalPadding,
                Settings.VerticalPadding,
                Settings.HorizontalPadding,
                Settings.VerticalPadding);

            control.Foreground = new SolidColorBrush(ConvertColor(Settings.FontColor));
            control.BorderThickness = new Thickness(Settings.BorderThickness);
            control.BorderBrush = new SolidColorBrush(ConvertColor(Settings.BorderColor));

            Settings.PropertyChanged += (S, E) =>
            {
                switch (E.PropertyName)
                {
                    case nameof(Settings.BackgroundColor):
                        control.Background = new SolidColorBrush(ConvertColor(Settings.BackgroundColor));
                        break;

                    case nameof(Settings.FontColor):
                        control.Foreground = new SolidColorBrush(ConvertColor(Settings.FontColor));
                        break;

                    case nameof(Settings.BorderThickness):
                        control.BorderThickness = new Thickness(Settings.BorderThickness);
                        break;

                    case nameof(Settings.BorderColor):
                        control.BorderBrush = new SolidColorBrush(ConvertColor(Settings.BorderColor));
                        break;

                    case nameof(Settings.FontSize):
                        control.FontSize = Settings.FontSize;
                        break;

                    case nameof(Settings.HorizontalPadding):
                    case nameof(Settings.VerticalPadding):
                        control.Padding = new Thickness(Settings.HorizontalPadding,
                            Settings.VerticalPadding,
                            Settings.HorizontalPadding,
                            Settings.VerticalPadding);
                        break;
                }
            };

            return control;
        }

        static Color ConvertColor(System.Drawing.Color C)
        {
            return Color.FromArgb(C.A, C.R, C.G, C.B);
        }

        LayerFrame Keystrokes(KeystrokesSettings Settings)
        {
            return Text(Settings, "Keystrokes");
        }

        readonly List<LayerFrame> _textOverlays = new List<LayerFrame>();
        readonly List<LayerFrame> _imageOverlays = new List<LayerFrame>();

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
                            control.Tag = setting.Text;
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
                            control.Tag = setting.Source;
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
            }
        }
        
        async void OnLoaded(object Sender, RoutedEventArgs RoutedEventArgs)
        {
            PlaceOverlays();

            await UpdateBackground();
        }

        async Task UpdateBackground()
        {
            var vm = ServiceProvider.Get<MainViewModel>();

            Bitmap bmp;

            switch (vm.VideoViewModel.SelectedVideoSource)
            {
                case WindowPickerItem _:
                case ScreenPickerItem _:
                case FullScreenItem _:
                    bmp = ScreenShot.Capture();
                    break;

                default:
                    bmp = await vm.GetScreenShot();
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
                MouseClick.Stroke = new SolidColorBrush(ToColor(Settings.BorderColor));
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
                MousePointer.Stroke = new SolidColorBrush(ToColor(Settings.BorderColor));
                MousePointer.Fill = new SolidColorBrush(ToColor(Settings.Color));
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

        Color ToColor(System.Drawing.Color C)
        {
            return Color.FromArgb(C.A, C.R, C.G, C.B);
        }

        Color GetClickColor(MouseButton Button)
        {
            var settings = ServiceProvider.Get<Settings>();

            switch (Button)
            {
                case MouseButton.Middle:
                    return ToColor(settings.Clicks.MiddleClickColor);

                case MouseButton.Right:
                    return ToColor(settings.Clicks.RightClickColor);
                    
                default:
                    return ToColor(settings.Clicks.Color);
            }
        }

        bool _dragging;

        void UpdateMouseClickPosition(MouseEventArgs E)
        {
            var position = E.GetPosition(Grid);

            MouseClick.Margin = new Thickness(position.X - MouseClick.ActualWidth / 2, position.Y - MouseClick.ActualHeight / 2, 0, 0);
        }

        void UIElement_OnMouseDown(object Sender, MouseButtonEventArgs E)
        {
            _dragging = true;

            UpdateMouseClickPosition(E);

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

        void UIElement_OnMouseMove(object Sender, MouseEventArgs E)
        {
            if (_dragging)
            {
                UpdateMouseClickPosition(E);
            }

            if (ServiceProvider.Get<Settings>().MousePointerOverlay.Display)
                MousePointer.Visibility = Visibility.Visible;

            var position = E.GetPosition(Grid);

            if (position.X <= 0 || position.Y <= 0)
            {
                MousePointer.Visibility = Visibility.Collapsed;

                return;
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
