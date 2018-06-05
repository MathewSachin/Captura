using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Microsoft.Win32;

namespace Captura
{
    public enum ImageEffect
    {
        None,
        Negative,
        Green,
        Red,
        Blue,
        Grayscale,
        Sepia
    }

    delegate void ModifyPixel(ref byte Red, ref byte Green, ref byte Blue);

    public class HistoryState
    {
        public ImageEffect Effect { get; set; }

        public int Brightness { get; set; }

        public int Contrast { get; set; }
    }

    public class ImageEditorViewModel : NotifyPropertyChanged
    {
        int _stride;
        byte[] _data;

        readonly Stack<HistoryState> _history = new Stack<HistoryState>();

        const int BrightnessStep = 10;
        const int ContrastStep = 10;

        public ICommand OpenCommand { get; }
        public DelegateCommand SaveCommand { get; }
        public DelegateCommand UndoCommand { get; }

        public DelegateCommand SetEffectCommand { get; }
        public DelegateCommand SetBrightnessCommand { get; }
        public DelegateCommand SetContrastCommand { get; }

        public ImageEditorViewModel()
        {
            OpenCommand = new DelegateCommand(Open);
            SaveCommand = new DelegateCommand(Save, false);
            UndoCommand = new DelegateCommand(Undo, false);

            SetEffectCommand = new DelegateCommand(async M =>
            {
                if (M is ImageEffect effect)
                {
                    UpdateHistory();

                    CurrentImageEffect = effect;

                    await Update();
                }
            }, false);

            SetBrightnessCommand = new DelegateCommand(async M =>
            {
                if (M is int i || M is string s && int.TryParse(s, out i))
                {
                    UpdateHistory();

                    if (i == 0)
                        _brightness = 0;
                    else if (i > 0)
                        _brightness += BrightnessStep;
                    else _brightness -= BrightnessStep;

                    await Update();
                }
            }, false);

            SetContrastCommand = new DelegateCommand(async M =>
            {
                if (M is int i || M is string s && int.TryParse(s, out i))
                {
                    UpdateHistory();

                    if (i == 0)
                        _contrastThreshold = 0;
                    else if (i > 0)
                        _contrastThreshold += ContrastStep;
                    else _contrastThreshold -= ContrastStep;

                    await Update();
                }
            }, false);
        }

        BitmapFrame _originalBmp;

        public BitmapFrame OriginalBitmap
        {
            get => _originalBmp;
            private set
            {
                _originalBmp = value;
                
                OnPropertyChanged();
            }
        }

        WriteableBitmap _editedBmp;

        public WriteableBitmap EditedBitmap
        {
            get => _editedBmp;
            private set
            {
                _editedBmp = value;
                
                OnPropertyChanged();
            }
        }

        ImageEffect _imageEffect = ImageEffect.None;

        public ImageEffect CurrentImageEffect
        {
            get => _imageEffect;
            private set
            {
                _imageEffect = value;
                
                OnPropertyChanged();
            }
        }

        void Negative(ref byte Red, ref byte Green, ref byte Blue)
        {
            Red = (byte)(255 - Red);
            Green = (byte)(255 - Green);
            Blue = (byte)(255 - Blue);
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
        double _contrastLevel;

        void Contrast(ref byte Red, ref byte Green, ref byte Blue)
        {
            void Apply(ref byte Byte)
            {
                var val = ((Byte / 255.0 - 0.5) * _contrastLevel + 0.5) * 255.0;

                if (val > 255)
                    Byte = 255;
                else if (val < 0)
                    Byte = 0;
                else Byte = (byte)val;
            }

            Apply(ref Red);
            Apply(ref Green);
            Apply(ref Blue);
        }

        ModifyPixel GetEffectFunction()
        {
            switch (_imageEffect)
            {
                case ImageEffect.Negative:
                    return Negative;

                case ImageEffect.Blue:
                    return Blue;

                case ImageEffect.Green:
                    return Green;

                case ImageEffect.Red:
                    return Red;

                case ImageEffect.Sepia:
                    return Sepia;

                case ImageEffect.Grayscale:
                    return Grayscale;

                default:
                    return null;
            }
        }

        void UpdateHistory()
        {
            _history.Push(new HistoryState
            {
                Brightness = _brightness,
                Contrast = _contrastThreshold,
                Effect = _imageEffect
            });

            UndoCommand.RaiseCanExecuteChanged(true);
        }

        async Task Update()
        {
            OriginalBitmap.CopyPixels(_data, _stride, 0);

            var effectFunction = GetEffectFunction();

            _contrastLevel = Math.Pow((100.0 + _contrastThreshold) / 100.0, 2);

            await Task.Run(() =>
            {
                Parallel.For(0, _data.Length / 4, I =>
                {
                    var i = I * 4;

                    effectFunction?.Invoke(ref _data[i + 2], ref _data[i + 1], ref _data[i]);

                    Brightness(ref _data[i + 2], ref _data[i + 1], ref _data[i]);

                    Contrast(ref _data[i + 2], ref _data[i + 1], ref _data[i]);
                });
            });

            EditedBitmap.WritePixels(new Int32Rect(0, 0, OriginalBitmap.PixelWidth, OriginalBitmap.PixelHeight), _data, _stride, 0);
        }

        void Open()
        {
            var ofd = new OpenFileDialog
            {
                Filter = "PNG Image|*.png",
                CheckFileExists = true,
                CheckPathExists = true
            };

            if (ofd.ShowDialog().GetValueOrDefault())
            {
                var decoder = BitmapDecoder.Create(new Uri(ofd.FileName),
                    BitmapCreateOptions.None, BitmapCacheOption.None);

                OriginalBitmap = decoder.Frames[0];

                _stride = OriginalBitmap.PixelWidth * (OriginalBitmap.Format.BitsPerPixel / 8);

                _data = new byte[_stride * OriginalBitmap.PixelHeight];

                EditedBitmap = new WriteableBitmap(OriginalBitmap);

                _history.Clear();
                UndoCommand.RaiseCanExecuteChanged(false);

                SaveCommand.RaiseCanExecuteChanged(true);
                SetEffectCommand.RaiseCanExecuteChanged(true);
                SetBrightnessCommand.RaiseCanExecuteChanged(true);
                SetContrastCommand.RaiseCanExecuteChanged(true);
            }
        }

        void Save()
        {
            var encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(EditedBitmap));

            var sfd = new SaveFileDialog
            {
                Filter = "PNG Image|*.png",
                DefaultExt = ".png",
                AddExtension = true
            };

            if (sfd.ShowDialog().GetValueOrDefault())
            {
                using (var stream = sfd.OpenFile())
                    encoder.Save(stream);
            }
        }

        async void Undo()
        {
            if (_history.Count == 0)
                return;

            var state = _history.Pop();

            _imageEffect = state.Effect;
            _brightness = state.Brightness;
            _contrastThreshold = state.Contrast;

            await Update();

            if (_history.Count == 0)
                UndoCommand.RaiseCanExecuteChanged(false);
        }
    }

    public partial class ImageEditorWindow
    {
        public ImageEditorWindow()
        {
            InitializeComponent();
        }

        void Exit(object Sender, RoutedEventArgs E)
        {
            Close();
        }
    }
}
