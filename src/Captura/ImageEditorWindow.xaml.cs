using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
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

    public delegate void ModifyPixel(ref byte Red, ref byte Green, ref byte Blue);

    public abstract class HistoryItem { }

    public class HistoryState : HistoryItem
    {
        public ImageEffect Effect { get; set; }

        public int Brightness { get; set; }

        public int Contrast { get; set; }

        public int Rotation { get; set; }

        public bool FlipX { get; set; }

        public bool FlipY { get; set; }
    }

    public class StrokeHistory : HistoryItem
    {
        public int EditingOperationCount { get; set; }

        public InkCanvasEditingMode EditingMode { get; set; }

        public List<Stroke> Added { get; } = new List<Stroke>();
        public List<Stroke> Removed { get; } = new List<Stroke>();
    }

    public static class PixelFunctionFactory
    {
        static void Negative(ref byte Red, ref byte Green, ref byte Blue)
        {
            Red = (byte)(255 - Red);
            Green = (byte)(255 - Green);
            Blue = (byte)(255 - Blue);
        }

        static void Green(ref byte Red, ref byte Green, ref byte Blue)
        {
            Red = Blue = 0;
        }

        static void Red(ref byte Red, ref byte Green, ref byte Blue)
        {
            Green = Blue = 0;
        }

        static void Blue(ref byte Red, ref byte Green, ref byte Blue)
        {
            Red = Green = 0;
        }

        static void Grayscale(ref byte Red, ref byte Green, ref byte Blue)
        {
            var pixel = 0.299 * Red + 0.587 * Green + 0.114 * Blue;

            if (pixel > 255)
                pixel = 255;

            Red = Green = Blue = (byte)pixel;
        }

        static void Sepia(ref byte Red, ref byte Green, ref byte Blue)
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

        public static ModifyPixel GetEffectFunction(ImageEffect Effect)
        {
            switch (Effect)
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
    }

    public class ImageEditorViewModel : NotifyPropertyChanged
    {
        int _stride;
        byte[] _data;

        int _editingOperationCount;

        readonly Stack<HistoryItem> _undoStack = new Stack<HistoryItem>();
        readonly Stack<HistoryItem> _redoStack = new Stack<HistoryItem>();

        const int BrightnessStep = 10;
        const int ContrastStep = 10;

        public ICommand OpenCommand { get; }
        public DelegateCommand UndoCommand { get; }
        public DelegateCommand RedoCommand { get; }

        public DelegateCommand SetEffectCommand { get; }
        public DelegateCommand SetBrightnessCommand { get; }
        public DelegateCommand SetContrastCommand { get; }

        public DelegateCommand RotateRightCommand { get; }
        public DelegateCommand RotateLeftCommand { get; }
        public DelegateCommand FlipXCommand { get; }
        public DelegateCommand FlipYCommand { get; }

        public ImageEditorViewModel()
        {
            OpenCommand = new DelegateCommand(Open);
            UndoCommand = new DelegateCommand(Undo, false);
            RedoCommand = new DelegateCommand(Redo, false);

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
                    {
                        if (_contrastThreshold == 100)
                            return;

                        _contrastThreshold += ContrastStep;
                    }
                    else
                    {
                        if (_contrastThreshold == -100)
                            return;

                        _contrastThreshold -= ContrastStep;
                    }

                    await Update();
                }
            }, false);

            RotateRightCommand = new DelegateCommand(async M =>
            {
                UpdateHistory();

                Rotation += 90;

                await Update();
            }, false);

            RotateLeftCommand = new DelegateCommand(async M =>
            {
                UpdateHistory();

                Rotation -= 90;

                await Update();
            }, false);

            FlipXCommand = new DelegateCommand(async M =>
            {
                UpdateHistory();

                FlipX = !FlipX;

                await Update();
            }, false);

            FlipYCommand = new DelegateCommand(async M =>
            {
                UpdateHistory();

                FlipY = !FlipY;

                await Update();
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

        TransformedBitmap _transformedBmp;

        public TransformedBitmap TransformedBitmap
        {
            get => _transformedBmp;
            set
            {
                _transformedBmp = value;
                
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

        int _brightness;

        void Brightness(ref byte Red, ref byte Green, ref byte Blue)
        {
            void Apply(ref byte Byte)
            {
                var val = Byte + _brightness;

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

        HistoryState GetHistoryState()
        {
            return new HistoryState
            {
                Brightness = _brightness,
                Contrast = _contrastThreshold,
                Effect = _imageEffect,
                Rotation = Rotation,
                FlipX = FlipX,
                FlipY = FlipY
            };
        }

        void UpdateHistory()
        {
            _undoStack.Push(GetHistoryState());

            UndoCommand.RaiseCanExecuteChanged(true);

            _redoStack.Clear();

            RedoCommand.RaiseCanExecuteChanged(false);
        }

        async Task Update()
        {
            OriginalBitmap.CopyPixels(_data, _stride, 0);

            var effectFunction = PixelFunctionFactory.GetEffectFunction(CurrentImageEffect);

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

            UpdateTransformBitmap();
        }

        public int Rotation { get; private set; }

        public bool FlipX { get; private set; }
        public bool FlipY { get; private set; }

        void UpdateTransformBitmap()
        {
            var rotate = new RotateTransform(Rotation, OriginalBitmap.PixelWidth / 2.0, OriginalBitmap.PixelHeight / 2.0);
            var scale = new ScaleTransform(FlipX ? -1 : 1, FlipY ? -1 : 1);

            TransformedBitmap = new TransformedBitmap(EditedBitmap, new TransformGroup
            {
                Children =
                {
                    rotate,
                    scale
                }
            });
        }

        void Reset()
        {
            _brightness = _contrastThreshold = 0;
            _imageEffect = ImageEffect.None;

            Rotation = 0;
            FlipX = FlipY = false;

            _undoStack.Clear();
            UndoCommand.RaiseCanExecuteChanged(false);
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
                Reset();

                var decoder = BitmapDecoder.Create(new Uri(ofd.FileName),
                    BitmapCreateOptions.None, BitmapCacheOption.None);

                OriginalBitmap = decoder.Frames[0];

                _stride = OriginalBitmap.PixelWidth * (OriginalBitmap.Format.BitsPerPixel / 8);

                _data = new byte[_stride * OriginalBitmap.PixelHeight];

                EditedBitmap = new WriteableBitmap(OriginalBitmap);
                
                UpdateTransformBitmap();

                SetEffectCommand.RaiseCanExecuteChanged(true);
                SetBrightnessCommand.RaiseCanExecuteChanged(true);
                SetContrastCommand.RaiseCanExecuteChanged(true);

                RotateRightCommand.RaiseCanExecuteChanged(true);
                RotateLeftCommand.RaiseCanExecuteChanged(true);
                FlipXCommand.RaiseCanExecuteChanged(true);
                FlipYCommand.RaiseCanExecuteChanged(true);
            }
        }

        public void Save(BitmapSource Bmp)
        {
            var encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(Bmp));

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

        public InkCanvas InkCanvas { get; set; }

        public void AddInkHistory(StrokeHistory HistoryItem)
        {
            if (!_trackChanges)
                return;

            HistoryItem.EditingMode = InkCanvas.EditingMode;
            HistoryItem.EditingOperationCount = _editingOperationCount;

            var merged = false;

            if (_undoStack.Count > 0)
            {
                var peek = _undoStack.Peek();

                if (HistoryItem.EditingMode == InkCanvasEditingMode.EraseByPoint
                    && peek is StrokeHistory stroke
                    && stroke.EditingMode == InkCanvasEditingMode.EraseByPoint
                    && HistoryItem.EditingOperationCount == stroke.EditingOperationCount)
                {
                    // Possible for point-erase to have hit intersection of >1 strokes!
                    // For each newly hit stroke, merge results into this history item.
                    foreach (var doomed in HistoryItem.Removed)
                    {
                        if (stroke.Added.Contains(doomed))
                        {
                            stroke.Added.Remove(doomed);
                        }
                        else
                        {
                            stroke.Removed.Add(doomed);
                        }
                    }

                    stroke.Added.AddRange(HistoryItem.Added);
                    
                    merged = true;
                }
            }

            if (!merged)
            {
                _undoStack.Push(HistoryItem);

                UndoCommand.RaiseCanExecuteChanged(true);
            }

            _redoStack.Clear();

            RedoCommand.RaiseCanExecuteChanged(false);
        }

        bool _trackChanges = true;

        async void Undo()
        {
            _trackChanges = false;

            if (_undoStack.Count == 0)
                return;

            var current = GetHistoryState();

            var item = _undoStack.Pop();

            if (item is HistoryState state)
            {
                ApplyHistoryState(state);

                _redoStack.Push(current);
            }
            else if (item is StrokeHistory strokes)
            {
                foreach (var stroke in strokes.Added)
                {
                    InkCanvas.Strokes.Remove(stroke);
                }

                foreach (var stroke in strokes.Removed)
                {
                    InkCanvas.Strokes.Add(stroke);
                }

                _redoStack.Push(item);
            }
            
            RedoCommand.RaiseCanExecuteChanged(true);

            await Update();

            if (_undoStack.Count == 0)
                UndoCommand.RaiseCanExecuteChanged(false);

            _trackChanges = true;
        }

        async void Redo()
        {
            _trackChanges = false;

            if (_redoStack.Count == 0)
                return;

            var current = GetHistoryState();

            var item = _redoStack.Pop();

            if (item is HistoryState state)
            {
                ApplyHistoryState(state);

                _undoStack.Push(current);
            }
            else if (item is StrokeHistory strokes)
            {
                foreach (var stroke in strokes.Added)
                {
                    InkCanvas.Strokes.Add(stroke);
                }

                foreach (var stroke in strokes.Removed)
                {
                    InkCanvas.Strokes.Remove(stroke);
                }

                _undoStack.Push(item);
            }
            
            UndoCommand.RaiseCanExecuteChanged(true);

            await Update();

            if (_redoStack.Count == 0)
                RedoCommand.RaiseCanExecuteChanged(false);

            _trackChanges = true;
        }

        void ApplyHistoryState(HistoryState State)
        {
            _imageEffect = State.Effect;
            _brightness = State.Brightness;
            _contrastThreshold = State.Contrast;
            Rotation = State.Rotation;
            FlipX = State.FlipX;
            FlipY = State.FlipY;
        }

        public IEnumerable<KeyValuePair<InkCanvasEditingMode, string>> Tools { get; } = new[]
        {
            new KeyValuePair<InkCanvasEditingMode, string>(InkCanvasEditingMode.Ink, "Pencil"),
            new KeyValuePair<InkCanvasEditingMode, string>(InkCanvasEditingMode.EraseByPoint, "Eraser"),
            new KeyValuePair<InkCanvasEditingMode, string>(InkCanvasEditingMode.EraseByStroke, "Stroke Eraser")
        };

        public void IncrementEditingOperationCount()
        {
            ++_editingOperationCount;
        }
    }

    public partial class ImageEditorWindow
    {
        public ImageEditorWindow()
        {
            InitializeComponent();

            if (DataContext is ImageEditorViewModel vm)
            {
                vm.PropertyChanged += (S, E) =>
                {
                    if (E.PropertyName == nameof(vm.TransformedBitmap))
                        UpdateInkCanvas();
                };

                vm.InkCanvas = InkCanvas;

                InkCanvas.Strokes.StrokesChanged += (S, E) =>
                {
                    var item = new StrokeHistory();
                    
                    item.Added.AddRange(E.Added);
                    item.Removed.AddRange(E.Removed);

                    vm.AddInkHistory(item);
                };
            }

            Image.SizeChanged += (S, E) => UpdateInkCanvas();

            ColorPicker.SelectedColor = Color.FromRgb(27, 27, 27);
            ModesBox.SelectedIndex = 0;
            SizeBox.Value = 10;

            InkCanvas.DefaultDrawingAttributes.FitToCurve = true;
        }

        void Exit(object Sender, RoutedEventArgs E)
        {
            Close();
        }

        void SizeBox_OnValueChanged(object Sender, RoutedPropertyChangedEventArgs<object> E)
        {
            if (InkCanvas != null && E.NewValue is int i)
                InkCanvas.DefaultDrawingAttributes.Height = InkCanvas.DefaultDrawingAttributes.Width = i;
        }

        void ModesBox_OnSelectionChanged(object Sender, SelectionChangedEventArgs E)
        {
            if (ModesBox.SelectedValue is InkCanvasEditingMode mode)
            {
                InkCanvas.EditingMode = mode;

                if (mode == InkCanvasEditingMode.Ink)
                {
                    InkCanvas.UseCustomCursor = true;
                    InkCanvas.Cursor = Cursors.Pen;
                }
                else InkCanvas.UseCustomCursor = false;
            }
        }

        void ColorPicker_OnSelectedColorChanged(object Sender, RoutedPropertyChangedEventArgs<Color?> E)
        {
            if (E.NewValue != null && InkCanvas != null)
                InkCanvas.DefaultDrawingAttributes.Color = E.NewValue.Value;
        }

        void UpdateInkCanvas()
        {
            if (DataContext is ImageEditorViewModel vm && vm.TransformedBitmap != null && Image.ActualWidth > 0)
            {
                InkCanvas.IsEnabled = true;

                InkCanvas.Width = vm.OriginalBitmap.PixelWidth;
                InkCanvas.Height = vm.OriginalBitmap.PixelHeight;

                var rotate = new RotateTransform(vm.Rotation, vm.OriginalBitmap.PixelWidth / 2.0, vm.OriginalBitmap.PixelHeight / 2.0);

                var tilted = Math.Abs(vm.Rotation / 90) % 2 == 1;
                
                var scale = new ScaleTransform(
                    ((tilted ? Image.ActualHeight : Image.ActualWidth) / InkCanvas.Width) * (vm.FlipX ? -1 : 1),
                    ((tilted ? Image.ActualWidth : Image.ActualHeight) / InkCanvas.Height) * (vm.FlipY ? -1 : 1)
                );

                InkCanvas.LayoutTransform = new TransformGroup
                {
                    Children =
                    {
                        rotate,
                        scale
                    }
                };
            }
        }

        void OnSave(object Sender, ExecutedRoutedEventArgs E)
        {
            if (DataContext is ImageEditorViewModel vm && vm.TransformedBitmap != null)
            {
                var drawingVisual = new DrawingVisual();

                var copy = vm.EditedBitmap;
                var transform = vm.TransformedBitmap.Transform;

                using (var drawingContext = drawingVisual.RenderOpen())
                {
                    drawingContext.DrawImage(copy, new Rect(0, 0, copy.Width, copy.Height));

                    InkCanvas.Strokes.Draw(drawingContext);

                    drawingContext.Close();

                    var bitmap = new RenderTargetBitmap((int)copy.Width,
                        (int)copy.Height,
                        copy.DpiX,
                        copy.DpiY,
                        PixelFormats.Pbgra32);

                    bitmap.Render(drawingVisual);

                    var transformedRendered = new TransformedBitmap(bitmap, transform);

                    vm.Save(transformedRendered);
                }
            }
        }

        void InkCanvas_OnMouseUp(object Sender, MouseButtonEventArgs E)
        {
            if (DataContext is ImageEditorViewModel vm)
            {
                vm.IncrementEditingOperationCount();
            }
        }
    }
}
