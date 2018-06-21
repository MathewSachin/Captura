using System.Collections.Generic;
using System.Collections.Specialized;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
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

            SizeChanged += (S, E) => UpdateSizeText();
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

        void UpdateSizeText()
        {
            Grid.Tag = $"{(int)Grid.ActualWidth} x {(int)Grid.ActualHeight}";
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

            Scale.ScaleX = scaleX;
            Scale.ScaleY = scaleY;
        }

        void PlaceOverlays()
        {
            UpdateSizeText();

            var settings = ServiceProvider.Get<Settings>();

            var keystrokes = Keystrokes(settings.Keystrokes);

            var webcam = Webcam(settings.WebcamOverlay);

            var textOverlayVm = ServiceProvider.Get<CustomOverlaysViewModel>();

            UpdateTextOverlays(textOverlayVm.Collection);
            (textOverlayVm.Collection as INotifyCollectionChanged).CollectionChanged += (S, E) => UpdateTextOverlays(textOverlayVm.Collection);

            var imgOverlayVm = ServiceProvider.Get<CustomImageOverlaysViewModel>();

            UpdateImageOverlays(imgOverlayVm.Collection);
            (imgOverlayVm.Collection as INotifyCollectionChanged).CollectionChanged += (S, E) => UpdateImageOverlays(imgOverlayVm.Collection);

            AddToGrid(keystrokes, false);
            AddToGrid(webcam, true);
        }

        void OverlayWindow_OnSizeChanged(object Sender, SizeChangedEventArgs E)
        {
            UpdateScale();
        }

        void Img_OnLoaded(object Sender, RoutedEventArgs E)
        {
            UpdateScale();
        }
    }
}
