using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Captura.ViewModels;

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
                ServiceProvider.Get<CustomOverlaysViewModel>().Dispose();
                ServiceProvider.Get<CustomImageOverlaysViewModel>().Dispose();
            };

            SizeChanged += (S, E) => UpdateSizeText();

            UpdateSelection(ItemsControl);

            UpdateSelection(ImagesItemsControl);
        }

        LayerFrame Generate(PositionedOverlaySettings Settings, string Name, int Width, int Height, Color Background)
        {
            var control = new LayerFrame
            {
                Width = Width,
                Height = Height,
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

            control.PositionUpdated += (X, Y, W, H) =>
            {
                Settings.X = (int)X;
                Settings.Y = (int)Y;
            };

            return control;
        }

        LayerFrame Webcam(WebcamOverlaySettings Settings)
        {
            var webcam = Generate(Settings, "Webcam",
                Settings.ResizeWidth,
                Settings.ResizeHeight,
                Colors.Brown);

            webcam.Opacity = Settings.Opacity / 100.0;

            Settings.PropertyChanged += (S, E) =>
            {
                Dispatcher.Invoke(() =>
                {
                    webcam.Width = Settings.ResizeWidth;
                    webcam.Height = Settings.ResizeHeight;
                    webcam.Opacity = Settings.Opacity / 100.0;
                });
            };

            webcam.PositionUpdated += (X, Y, W, H) =>
            {
                Settings.ResizeWidth = (int)W;
                Settings.ResizeHeight = (int)H;
            };
            
            return webcam;
        }

        LayerFrame Keystrokes(KeystrokesSettings Settings)
        {
            Color ConvertColor(System.Drawing.Color C)
            {
                return Color.FromArgb(C.A, C.R, C.G, C.B);
            }

            var keystrokes = Generate(Settings, "Keystrokes", 200, 50, ConvertColor(Settings.BackgroundColor));
            keystrokes.Width = double.NaN;
            keystrokes.Height = double.NaN;

            keystrokes.FontSize = Settings.FontSize;

            keystrokes.Padding = new Thickness(Settings.HorizontalPadding,
                Settings.VerticalPadding,
                Settings.HorizontalPadding,
                Settings.VerticalPadding);

            keystrokes.Foreground = new SolidColorBrush(ConvertColor(Settings.FontColor));
            keystrokes.BorderThickness = new Thickness(Settings.BorderThickness);
            keystrokes.BorderBrush = new SolidColorBrush(ConvertColor(Settings.BorderColor));

            Settings.PropertyChanged += (S, E) =>
            {
                switch (E.PropertyName)
                {
                    case nameof(Settings.BackgroundColor):
                        keystrokes.Background = new SolidColorBrush(ConvertColor(Settings.BackgroundColor));
                        break;

                    case nameof(Settings.FontColor):
                        keystrokes.Foreground = new SolidColorBrush(ConvertColor(Settings.FontColor));
                        break;

                    case nameof(Settings.BorderThickness):
                        keystrokes.BorderThickness = new Thickness(Settings.BorderThickness);
                        break;

                    case nameof(Settings.BorderColor):
                        keystrokes.BorderBrush = new SolidColorBrush(ConvertColor(Settings.BorderColor));
                        break;

                    case nameof(Settings.FontSize):
                        keystrokes.FontSize = Settings.FontSize;
                        break;

                    case nameof(Settings.HorizontalPadding):
                    case nameof(Settings.VerticalPadding):
                        keystrokes.Padding = new Thickness(Settings.HorizontalPadding,
                            Settings.VerticalPadding,
                            Settings.HorizontalPadding,
                            Settings.VerticalPadding);
                        break;
                }
            };

            return keystrokes;
        }

        void UpdateSizeText()
        {
            Grid.Tag = $"{(int)Grid.ActualWidth} x {(int)Grid.ActualHeight}";
        }

        void OnLoaded(object Sender, RoutedEventArgs RoutedEventArgs)
        {
            UpdateSizeText();

            var settings = ServiceProvider.Get<Settings>();

            var keystrokes = Keystrokes(settings.Keystrokes);

            var webcam = Webcam(settings.WebcamOverlay);

            Grid.Children.Add(keystrokes);
            Grid.Children.Add(webcam);
        }

        void ItemsControl_OnSelectionChanged(object Sender, SelectionChangedEventArgs E)
        {
            if (Sender is ListView listView)
                UpdateSelection(listView);
        }

        void UpdateSelection(ListView Sender)
        {
            if (Sender.SelectedIndex == -1 && Sender.HasItems)
            {
                Sender.SelectedIndex = 0;
            }
        }
    }
}
