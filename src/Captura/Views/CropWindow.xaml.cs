using System.IO;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Microsoft.Win32;

namespace Captura
{
    public partial class CropWindow
    {
        RubberbandAdorner _cropSelector;

        public CropWindow(string FileName)
        {
            InitializeComponent();

            var decoder = new PngBitmapDecoder(File.OpenRead(FileName), BitmapCreateOptions.None, BitmapCacheOption.Default);

            Image.Source = decoder.Frames[0];

            Loaded += (S, E) =>
            {
                var layer = AdornerLayer.GetAdornerLayer(Image);

                _cropSelector = new RubberbandAdorner(Image);

                _cropSelector.EnableCrop += () => CropButton.IsEnabled = true;

                layer.Add(_cropSelector);
            };
        }

        void Image_OnMouseDown(object Sender, MouseButtonEventArgs E)
        {
            CropButton.IsEnabled = false;

            var anchor = E.GetPosition(Image);

            _cropSelector.CaptureMouse();
            _cropSelector.StartSelection(anchor);
        }

        void Crop(object Sender, RoutedEventArgs E)
        {
            var img = (BitmapSource) Image.Source;

            var rect = new Int32Rect
            {
                X = (int) (_cropSelector.SelectRect.X * img.PixelWidth / Image.ActualWidth),
                Y = (int) (_cropSelector.SelectRect.Y * img.PixelHeight / Image.ActualHeight),
                Width = (int) (_cropSelector.SelectRect.Width * img.PixelWidth / Image.ActualWidth),
                Height = (int) (_cropSelector.SelectRect.Height * img.PixelHeight / Image.ActualHeight)
            };

            Image.Source = new CroppedBitmap(img, rect);

            _cropSelector.Rubberband.Visibility = Visibility.Hidden;

            CropButton.IsEnabled = false;

            _lastSource = img;

            UndoButton.IsEnabled = true;
        }

        BitmapSource _lastSource;

        void Undo(object Sender, RoutedEventArgs E)
        {
            if (_lastSource != null)
            {
                Image.Source = _lastSource;

                UndoButton.IsEnabled = false;
            }
        }

        void Save(object Sender, RoutedEventArgs E)
        {
            if (!(Image.Source is BitmapSource bmpSource))
                return;

            var sfd = new SaveFileDialog
            {
                AddExtension = true,
                DefaultExt = ".png",
                Filter = "PNG File|*.png",
                FileName = "Untitled.png"
            };

            if (sfd.ShowDialog().GetValueOrDefault())
            {
                var encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(bmpSource));

                using (var stream = sfd.OpenFile())
                {
                    encoder.Save(stream);
                }
            }
        }
    }
}
