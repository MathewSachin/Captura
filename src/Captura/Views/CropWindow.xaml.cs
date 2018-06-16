using System.IO;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Win32;

namespace Captura
{
    public partial class CropWindow
    {
        CroppingAdorner _croppingAdorner;
        BitmapSource _croppedImage;
        readonly string _fileName;

        public CropWindow(string FileName)
        {
            InitializeComponent();

            _fileName = FileName;

            using (var stream = File.OpenRead(FileName))
            {
                var decoder = BitmapDecoder.Create(stream, BitmapCreateOptions.None, BitmapCacheOption.OnLoad);

                Image.Source = decoder.Frames[0];
            }

            Loaded += (S, E) =>
            {
                var rcInterior = new Rect(
                    Image.ActualWidth * 0.2,
                    Image.ActualHeight * 0.2,
                    Image.ActualWidth * 0.6,
                    Image.ActualHeight * 0.6);

                var layer = AdornerLayer.GetAdornerLayer(Image);

                _croppingAdorner = new CroppingAdorner(Image, rcInterior);

                layer.Add(_croppingAdorner);
                
                void RefreshCropImage()
                {
                    _croppedImage = _croppingAdorner.BpsCrop(Image.Source as BitmapSource);

                    SizeLabel.Content = _croppedImage != null
                        ? $"{(int) _croppedImage.Width} x {(int) _croppedImage.Height}"
                        : "";
                }

                RefreshCropImage();

                _croppingAdorner.CropChanged += (Sender, Args) => RefreshCropImage();

                _croppingAdorner.Checked += Save;

                var clr = Colors.Black;
                clr.A = 110;
                _croppingAdorner.Fill = new SolidColorBrush(clr);
            };
        }
        
        void Save()
        {
            if (_croppedImage == null)
                return;

            var sfd = new SaveFileDialog
            {
                AddExtension = true,
                DefaultExt = ".png",
                Filter = "PNG Image|*.png|JPEG Image|*.jpg;*.jpeg|Bitmap Image|*.bmp|TIFF Image|*.tiff",
                InitialDirectory = Path.GetDirectoryName(_fileName),
                FileName = Path.GetFileNameWithoutExtension(_fileName)
            };

            if (sfd.ShowDialog().GetValueOrDefault())
            {
                BitmapEncoder encoder;

                // Filter Index starts from 1
                switch (sfd.FilterIndex)
                {
                    case 2:
                        encoder = new JpegBitmapEncoder();
                        break;

                    case 3:
                        encoder = new BmpBitmapEncoder();
                        break;

                    case 4:
                        encoder = new TiffBitmapEncoder();
                        break;

                    default:
                        encoder = new PngBitmapEncoder();
                        break;
                }

                encoder.Frames.Add(BitmapFrame.Create(_croppedImage));

                using (var stream = sfd.OpenFile())
                {
                    encoder.Save(stream);
                }

                Close();
            }
        }
    }
}
