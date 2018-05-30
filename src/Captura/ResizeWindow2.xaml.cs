using System.Windows;
using System.Windows.Media;

namespace Captura
{
    public partial class ResizeWindow2
    {
        public ResizeWindow2()
        {
            InitializeComponent();

            Loaded += OnLoaded;

            Closing += (S, E) => ServiceProvider.Get<Settings>().Save();
        }

        LayerFrame2 Generate(PositionedOverlaySettings Settings, string Name, int Width, int Height, Color Background)
        {
            var control = new LayerFrame2
            {
                Width = Width,
                Height = Height,
                Tag = Name,
                Background = new SolidColorBrush(Background),
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

        LayerFrame2 Webcam(WebcamOverlaySettings Settings)
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

        void OnLoaded(object Sender, RoutedEventArgs RoutedEventArgs)
        {
            var settings = ServiceProvider.Get<Settings>();

            var keystrokes = Generate(settings.Keystrokes, "Keystrokes", 200, 50, Colors.Crimson);

            var webcam = Webcam(settings.WebcamOverlay);

            Grid.Children.Add(keystrokes);
            Grid.Children.Add(webcam);
        }
    }
}
