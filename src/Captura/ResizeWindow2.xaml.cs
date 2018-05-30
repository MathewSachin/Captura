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
        }

        void OnLoaded(object Sender, RoutedEventArgs RoutedEventArgs)
        {
            var settings = ServiceProvider.Get<Settings>();

            var keySettings = settings.Keystrokes;

            var keystrokes = new LayerFrame2
            {
                Width = 200,
                Height = 50,
                Background = new SolidColorBrush(Colors.Aquamarine),
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top
            };

            void KeyUpdate()
            {
                int left = 0, top = 0, right = 0, bottom = 0;

                switch (keySettings.HorizontalAlignment)
                {
                    case Alignment.Start:
                        keystrokes.HorizontalAlignment = HorizontalAlignment.Left;
                        left = keySettings.X;
                        break;

                    case Alignment.Center:
                        keystrokes.HorizontalAlignment = HorizontalAlignment.Center;
                        left = keySettings.X;
                        break;

                    case Alignment.End:
                        keystrokes.HorizontalAlignment = HorizontalAlignment.Right;
                        right = keySettings.X;
                        break;
                }

                switch (keySettings.VerticalAlignment)
                {
                    case Alignment.Start:
                        keystrokes.VerticalAlignment = VerticalAlignment.Top;
                        top = keySettings.Y;
                        break;

                    case Alignment.Center:
                        keystrokes.VerticalAlignment = VerticalAlignment.Center;
                        top = keySettings.Y;
                        break;

                    case Alignment.End:
                        keystrokes.VerticalAlignment = VerticalAlignment.Bottom;
                        bottom = keySettings.Y;
                        break;
                }

                Dispatcher.Invoke(() => keystrokes.Margin = new Thickness(left, top, right, bottom));
            }

            keySettings.PropertyChanged += (S, E) => KeyUpdate();

            KeyUpdate();

            keystrokes.PositionUpdated += (X, Y) =>
            {
                keySettings.X = (int)X;
                keySettings.Y = (int)Y;
            };

            Grid.Children.Add(keystrokes);
        }
    }
}
