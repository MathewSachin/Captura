using System;
using System.Threading.Tasks;
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

        async Task ApplyEffect(Action EffectFunction)
        {
            _imgSource.CopyPixels(_data, _stride, 0);

            await Task.Run(EffectFunction);

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
                var pixel = 0.299 * _data[i + 2] + 0.587 * _data[i + 1] + 0.114 * _data[i];

                if (pixel > 255)
                    pixel = 255;

                _data[i] = _data[i + 1] = _data[i + 2] = (byte)pixel;
            }
        }

        void Sepia()
        {
            for (var i = 0; i < _data.Length; i += 4)
            {
                var blue = _data[i];
                var green = _data[i + 1];
                var red = _data[i + 2];

                var newRed = 0.393 * red + 0.769 * green + 0.189 * blue;
                var newGreen = 0.349 * red + 0.686 * green + 0.168 * blue;
                var newBlue = 0.272 * red + 0.534 * green + 0.131 * blue;

                // Red
                _data[i + 2] = (byte)(newRed > 255 ? 255 : newRed);

                // Green
                _data[i + 1] = (byte)(newGreen > 255 ? 255 : newGreen);

                // Blue
                _data[i] = (byte)(newBlue > 255 ? 255 : newBlue);
            }
        }

        int _brightness;

        void Brightness()
        {
            for (var i = 0; i < _data.Length; i += 4)
            {
                for (var j = 0; j < 3; ++j)
                {
                    var val = _data[i + j] + _brightness;
                    
                    _data[i + j] = (byte)(val > 255 ? 255 : val);
                }
            }
        }

        int _contrastThreshold;

        void Contrast()
        {
            var contastLevel = Math.Pow((100.0 + _contrastThreshold) / 100.0, 2);

            for (var i = 0; i < _data.Length; i += 4)
            {
                var blue = ((_data[i] / 255.0 - 0.5) * contastLevel + 0.5) * 255.0;
                var green = ((_data[i + 1] / 255.0 - 0.5) * contastLevel + 0.5) * 255.0;
                var red = ((_data[i + 2] / 255.0 - 0.5) * contastLevel + 0.5) * 255.0;

                byte Clip(double Val)
                {
                    if (Val > 255)
                        return 255;

                    if (Val < 0)
                        return 0;

                    return (byte) Val;
                }

                _data[i] = Clip(blue);
                _data[i + 1] = Clip(green);
                _data[i + 2] = Clip(red);
            }
        }

        async void SepiaClick(object Sender, RoutedEventArgs E)
        {
            await ApplyEffect(Sepia);
        }

        async void GrayscaleClick(object Sender, RoutedEventArgs E)
        {
            await ApplyEffect(Grayscale);
        }

        async void NegativeClick(object Sender, RoutedEventArgs E)
        {
            await ApplyEffect(Negative);
        }

        async void RedClick(object Sender, RoutedEventArgs E)
        {
            await ApplyEffect(Red);
        }

        async void GreenClick(object Sender, RoutedEventArgs E)
        {
            await ApplyEffect(Green);
        }

        async void BlueClick(object Sender, RoutedEventArgs E)
        {
            await ApplyEffect(Blue);
        }

        const int BrightnessStep = 10;

        async void IncreaseBrightnessClick(object Sender, RoutedEventArgs E)
        {
            _brightness += BrightnessStep;

            await ApplyEffect(Brightness);
        }

        async void DecreaseBrightnessClick(object Sender, RoutedEventArgs E)
        {
            _brightness -= BrightnessStep;

            await ApplyEffect(Brightness);
        }

        async void ResetBrightnessClick(object Sender, RoutedEventArgs E)
        {
            _brightness = 0;

            await ApplyEffect(Brightness);
        }

        async void IncreaseContrastClick(object Sender, RoutedEventArgs E)
        {
            if (_contrastThreshold == 100)
                return;

            _contrastThreshold += 10;

            await ApplyEffect(Contrast);
        }

        async void DecreaseContrastClick(object Sender, RoutedEventArgs E)
        {
            if (_contrastThreshold == -100)
                return;

            _contrastThreshold -= 10;

            await ApplyEffect(Contrast);
        }

        async void ResetContrastClick(object Sender, RoutedEventArgs E)
        {
            _contrastThreshold = 0;

            await ApplyEffect(Contrast);
        }
    }
}
