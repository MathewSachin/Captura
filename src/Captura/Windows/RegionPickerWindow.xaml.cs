using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Screna;
using Color = System.Windows.Media.Color;
using HorizontalAlignment = System.Windows.HorizontalAlignment;
using MouseEventArgs = System.Windows.Input.MouseEventArgs;

namespace Captura
{
    public partial class RegionPickerWindow
    {
        RegionPickerWindow()
        {
            InitializeComponent();

            Left = SystemParameters.VirtualScreenLeft;
            Top = SystemParameters.VirtualScreenTop;
            Width = SystemParameters.VirtualScreenWidth;
            Height = SystemParameters.VirtualScreenHeight;

            UpdateBackground();

            ShowCancelText();
        }

        public Rectangle? SelectedRegion { get; private set; }

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
            foreach (var screen in Screen.AllScreens)
            {
                var left = -Left + screen.Bounds.Left / Dpi.X;
                var top = -Top + screen.Bounds.Top / Dpi.Y;
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
                    Text = $"Drag to Select Region or Press Esc to Cancel",
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    Padding = new Thickness(10, 5, 10, 5),
                    Foreground = new SolidColorBrush(Colors.White),
                    Background = new SolidColorBrush(Color.FromArgb(183, 0, 0, 0))
                };

                container.Content = textBlock;

                Grid.Children.Add(container);
            }
        }

        void CloseClick(object Sender, RoutedEventArgs E)
        {
            SelectedRegion = null;

            Close();
        }

        void WindowMouseMove(object Sender, MouseEventArgs E)
        {
        }

        void WindowMouseLeftButtonDown(object Sender, MouseButtonEventArgs E)
        {
        }

        public static Rectangle? PickRegion()
        {
            var picker = new RegionPickerWindow();

            picker.ShowDialog();

            return picker.SelectedRegion;
        }
    }
}
