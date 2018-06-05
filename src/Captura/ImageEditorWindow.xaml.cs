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

        delegate void ModifyPixel(ref byte Red, ref byte Green, ref byte Blue);

        async Task ApplyEffect(ModifyPixel EffectFunction)
        {
            _imgSource.CopyPixels(_data, _stride, 0);

            await Task.Run(() =>
            {
                for (var i = 0; i < _data.Length; i += 4)
                {
                    EffectFunction(ref _data[i + 2], ref _data[i + 1], ref _data[i]);
                }
            });

            _writableBmp.WritePixels(new Int32Rect(0, 0, _imgSource.PixelWidth, _imgSource.PixelHeight), _data, _stride, 0);
        }

        void Negative(ref byte Red, ref byte Green, ref byte Blue)
        {
            Red = (byte) (255 - Red);
            Green = (byte) (255 - Green);
            Blue = (byte) (255 - Blue);
        }

        void Green(ref byte Red, ref byte Green, ref byte Blue)
        {
            Red = Blue = 0;
        }

        void Red(ref byte Red, ref byte Green, ref byte Blue)
        {
            Green = Blue = 0;
        }

        void Blue(ref byte Red, ref byte Green, ref byte Blue)
        {
            Red = Green = 0;
        }

        void Grayscale(ref byte Red, ref byte Green, ref byte Blue)
        {
            var pixel = 0.299 * Red + 0.587 * Green + 0.114 * Blue;

            if (pixel > 255)
                pixel = 255;

            Red = Green = Blue = (byte)pixel;
        }

        void Sepia(ref byte Red, ref byte Green, ref byte Blue)
        {
            var newRed = 0.393 * Red + 0.769 * Green + 0.189 * Blue;
            var newGreen = 0.349 * Red + 0.686 * Green + 0.168 * Blue;
            var newBlue = 0.272 * Red + 0.534 * Green + 0.131 * Blue;

            // Red
            Red = (byte)(newRed > 255 ? 255 : newRed);

            // Green
            Green = (byte)(newGreen > 255 ? 255 : newGreen);

            // Blue
            Blue = (byte)(newBlue > 255 ? 255 : newBlue);
        }

        int _brightness;

        void Brightness(ref byte Red, ref byte Green, ref byte Blue)
        {
            void Apply(ref byte Byte)
            {
                var val = Byte + _brightness;

                Byte = (byte)(val > 255 ? 255 : val);
            }

            Apply(ref Red);
            Apply(ref Green);
            Apply(ref Blue);
        }

        int _contrastThreshold;

        void Contrast(ref byte Red, ref byte Green, ref byte Blue)
        {
            var contastLevel = Math.Pow((100.0 + _contrastThreshold) / 100.0, 2);

            void Apply(ref byte Byte)
            {
                var val = ((Byte / 255.0 - 0.5) * contastLevel + 0.5) * 255.0;

                if (val > 255)
                    Byte = 255;
                else if (val < 0)
                    Byte = 0;
                else Byte = (byte) val;
            }
            
            Apply(ref Red);
            Apply(ref Green);
            Apply(ref Blue);
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
