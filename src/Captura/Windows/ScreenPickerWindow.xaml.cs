using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Captura.Models;
using Screna;

namespace Captura
{
    public partial class ScreenPickerWindow
    {
        const double Scale = 0.15;

        ScreenPickerWindow()
        {
            InitializeComponent();

            var left = SystemParameters.VirtualScreenLeft * Scale;
            var top = SystemParameters.VirtualScreenTop * Scale;

            Container.Width = left + SystemParameters.VirtualScreenWidth * Scale;
            Container.Height = top + SystemParameters.VirtualScreenHeight * Scale;

            var platformServices = ServiceProvider.Get<IPlatformServices>();

            var screens = platformServices.EnumerateScreens().ToArray();

            foreach (var screen in screens)
            {
                using (var bmp = ScreenShot.Capture(screen.Rectangle))
                {
                    var stream = new MemoryStream();
                    bmp.Save(stream, ImageFormats.Png);

                    stream.Seek(0, SeekOrigin.Begin);

                    var decoder = new PngBitmapDecoder(stream, BitmapCreateOptions.None, BitmapCacheOption.Default);
                    var brush = new ImageBrush(decoder.Frames[0]);

                    var btn = new Button
                    {
                        Background = brush,
                        Cursor = Cursors.Hand,
                        HorizontalContentAlignment = HorizontalAlignment.Stretch,
                        Padding = new Thickness(),
                        Content = new Label
                        {
                            Background = new SolidColorBrush(Color.FromArgb(183, 0, 0, 0)),
                            Foreground = new SolidColorBrush(Colors.White),
                            Content = screen.DeviceName,
                            HorizontalContentAlignment = HorizontalAlignment.Center,
                            Margin = new Thickness(0, 0, 0, 10)
                        }
                    };

                    var border = new Border
                    {
                        Width = screen.Rectangle.Width / Dpi.X * Scale,
                        Height = screen.Rectangle.Height / Dpi.Y * Scale,
                        BorderThickness = new Thickness(2),
                        BorderBrush = new SolidColorBrush(Colors.Chocolate),
                        Child = btn
                    };

                    Container.Children.Add(border);

                    Canvas.SetLeft(btn, screen.Rectangle.Left / Dpi.X * Scale - left);
                    Canvas.SetTop(btn, screen.Rectangle.Top / Dpi.Y * Scale - top);

                    btn.Click += (S, E) =>
                    {
                        SelectedScreen = screen;

                        Close();
                    };
                }
            }
        }

        public IScreen SelectedScreen { get; private set; }

        void CloseClick(object Sender, RoutedEventArgs E)
        {
            SelectedScreen = null;

            Close();
        }

        public static IScreen PickScreen()
        {
            var picker = new ScreenPickerWindow
            {
                Owner = MainWindow.Instance
            };

            picker.ShowDialog();

            return picker.SelectedScreen;
        }
    }
}
