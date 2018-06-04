using System;
using System.Windows;
using System.Windows.Media.Imaging;

namespace Captura
{
    public partial class ImageEditorWindow
    {
        readonly BitmapFrame _imgSource;
        readonly int _stride;
        readonly byte[] _data; // BGRA
        readonly WriteableBitmap _writableBmp;

        public ImageEditorWindow()
        {
            InitializeComponent();

            var decoder = BitmapDecoder.Create(new Uri(@"C:\Users\Mathew\Pictures\FranXX\Genista.png"),
                BitmapCreateOptions.None, BitmapCacheOption.None);

            _imgSource = decoder.Frames[0];

            _stride = _imgSource.PixelWidth * (_imgSource.Format.BitsPerPixel / 8);

            _data = new byte[_stride * _imgSource.PixelHeight];

            _writableBmp = new WriteableBitmap(_imgSource);

            Img.Source = _writableBmp;
        }

        void ApplyEffect(Action EffectFunction)
        {
            _imgSource.CopyPixels(_data, _stride, 0);

            EffectFunction?.Invoke();

            _writableBmp.WritePixels(new Int32Rect(0, 0, _imgSource.PixelWidth, _imgSource.PixelHeight), _data, _stride, 0);
        }

        void Negative()
        {
            for (var i = 0; i < _data.Length; i += 4)
            {
                for (var j = 0; j < 3; ++j)
                    _data[i + j] = (byte)(255 - _data[i + j]);
            }
        }

        void Green()
        {
            for (var i = 0; i < _data.Length; i += 4)
            {
                _data[i] = 0;
                _data[i + 2] = 0;
            }
        }

        void Red()
        {
            for (var i = 0; i < _data.Length; i += 4)
            {
                _data[i] = 0;
                _data[i + 1] = 0;
            }
        }

        void Blue()
        {
            for (var i = 0; i < _data.Length; i += 4)
            {
                _data[i + 1] = 0;
                _data[i + 2] = 0;
            }
        }

        void Grayscale()
        {
            for (var i = 0; i < _data.Length; i += 4)
            {
                var pixel = (byte)(0.299 * _data[i + 2] + 0.587 * _data[i + 1] + 0.114 * _data[i]);

                _data[i] = _data[i + 1] = _data[i + 2] = pixel;
            }
        }

        void Sepia()
        {
            for (var i = 0; i < _data.Length; i += 4)
            {
                var blue = _data[i];
                var green = _data[i + 1];
                var red = _data[i + 2];

                // Red
                _data[i + 2] = (byte)(0.393 * red + 0.769 * green + 0.189 * blue);

                // Green
                _data[i + 1] = (byte)(0.349 * red + 0.686 * green + 0.168 * blue);

                // Blue
                _data[i] = (byte)(0.272 * red + 0.534 * green + 0.131 * blue);
            }
        }

        void SepiaClick(object Sender, RoutedEventArgs E)
        {
            ApplyEffect(Sepia);
        }

        void GrayscaleClick(object Sender, RoutedEventArgs E)
        {
            ApplyEffect(Grayscale);
        }

        void NegativeClick(object Sender, RoutedEventArgs E)
        {
            ApplyEffect(Negative);
        }

        void RedClick(object Sender, RoutedEventArgs E)
        {
            ApplyEffect(Red);
        }

        void GreenClick(object Sender, RoutedEventArgs E)
        {
            ApplyEffect(Green);
        }

        void BlueClick(object Sender, RoutedEventArgs E)
        {
            ApplyEffect(Blue);
        }
    }
}
