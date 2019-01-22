using System.IO;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Captura
{
    public partial class CropWindow
    {
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

                var croppingAdorner = new CroppingAdorner(Image, rcInterior);

                layer.Add(croppingAdorner);
                
                void RefreshCropImage()
                {
                    _croppedImage = croppingAdorner.BpsCrop(Image.Source as BitmapSource);

                    SizeLabel.Content = _croppedImage != null
                        ? $"{(int) _croppedImage.Width} x {(int) _croppedImage.Height}"
                        : "";
                }

                RefreshCropImage();

                croppingAdorner.CropChanged += (Sender, Args) => RefreshCropImage();

                croppingAdorner.Checked += Save;

                var clr = Colors.Black;
                clr.A = 110;
                croppingAdorner.Fill = new SolidColorBrush(clr);
            };
        }
        
        void Save()
        {
            if (_croppedImage == null)
                return;

            if (_croppedImage.SaveToPickedFile(_fileName))
            {
                Close();
            }
        }
    }
}
