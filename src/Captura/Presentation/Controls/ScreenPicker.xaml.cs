using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Screna;
using Cursors = System.Windows.Input.Cursors;
using HorizontalAlignment = System.Windows.HorizontalAlignment;
using MouseEventArgs = System.Windows.Input.MouseEventArgs;
using Point = System.Drawing.Point;

namespace Captura
{
    public partial class ScreenPicker
    {
        public ScreenPicker()
        {
            InitializeComponent();

            Left = Top = 0;
            Width = SystemParameters.VirtualScreenWidth;
            Height = SystemParameters.VirtualScreenHeight;

            UpdateBackground();

            _screens = Screen.AllScreens;

            ShowCancelText();
        }

        readonly Screen[] _screens;

        public Screen SelectedScreen { get; private set; }

        void UpdateBackground()
        {
            using (var bmp = ScreenShot.Capture())
            {
                var stream = new MemoryStream();
                bmp.Save(stream, ImageFormat.Png);

                stream.Seek(0, SeekOrigin.Begin);

                var decoder = new PngBitmapDecoder(stream, BitmapCreateOptions.None, BitmapCacheOption.Default);
                Background = new ImageBrush(decoder.Frames[0]);
            }
        }

        void ShowCancelText()
        {
            foreach (var screen in _screens)
            {
                var left = screen.Bounds.Left / Dpi.X;
                var top = screen.Bounds.Top / Dpi.Y;
                var width = screen.Bounds.Width / Dpi.X;
                var height = screen.Bounds.Height / Dpi.Y;

                var container = new ContentControl
                {
                    Width = width,
                    Height = height,
                    Margin = new Thickness(left, top, 0, 0),
                    VerticalAlignment = VerticalAlignment.Top,
                    HorizontalAlignment = HorizontalAlignment.Left
                };

                var textBlock = new TextBlock
                {
                    Text = "Select Screen or Press Esc to Cancel",
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    Foreground = new SolidColorBrush(Colors.Black)
                };

                container.Content = textBlock;

                Grid.Children.Add(container);
            }
        }

        void CloseClick(object Sender, RoutedEventArgs E)
        {
            SelectedScreen = null;

            Close();
        }

        void WindowMouseMove(object Sender, MouseEventArgs E)
        {
            var pos = E.GetPosition(this);

            var point = new Point((int) (pos.X * Dpi.X), (int) (pos.Y * Dpi.Y));

            var screen = _screens.FirstOrDefault(M => M.Bounds.Contains(point));

            if (screen != null)
            {
                SelectedScreen = screen;

                Cursor = Cursors.Hand;

                var rect = screen.Bounds;

                ScreenBorder.Margin = new Thickness(rect.Left / Dpi.X, rect.Top / Dpi.Y, 0, 0);

                ScreenBorder.Width = rect.Width / Dpi.X;
                ScreenBorder.Height = rect.Height / Dpi.Y;
                
                ScreenBorder.Visibility = Visibility.Visible;
            }
            else
            {
                SelectedScreen = null;

                Cursor = Cursors.Arrow;

                ScreenBorder.Visibility = Visibility.Collapsed;
            }
        }

        void WindowMouseLeftButtonDown(object Sender, MouseButtonEventArgs E)
        {
            if (SelectedScreen != null)
            {
                Close();
            }
        }
    }
}
