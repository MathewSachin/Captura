using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using FirstFloor.ModernUI.Windows.Controls;
using Microsoft.Win32;

namespace Captura
{
    public class ImageEditorViewModel : NotifyPropertyChanged
    {
        int _stride;
        byte[] _data;
        public Window Window { get; set; }

        public bool UnsavedChanges { get; private set; }

        int _editingOperationCount;

        readonly Stack<HistoryItem> _undoStack = new Stack<HistoryItem>();
        readonly Stack<HistoryItem> _redoStack = new Stack<HistoryItem>();

        const int BrightnessStep = 10;
        const int ContrastStep = 10;

        public DelegateCommand DiscardChangesCommand { get; }
        public DelegateCommand UndoCommand { get; }
        public DelegateCommand RedoCommand { get; }
        public DelegateCommand SaveCommand { get; }
        public DelegateCommand SaveToClipboardCommand { get; }

        public DelegateCommand SetEffectCommand { get; }
        public DelegateCommand SetBrightnessCommand { get; }
        public DelegateCommand SetContrastCommand { get; }

        public DelegateCommand RotateRightCommand { get; }
        public DelegateCommand RotateLeftCommand { get; }
        public DelegateCommand FlipXCommand { get; }
        public DelegateCommand FlipYCommand { get; }

        public ImageEditorViewModel()
        {
            UndoCommand = new DelegateCommand(Undo, false);
            RedoCommand = new DelegateCommand(Redo, false);
            SaveCommand = new DelegateCommand(() => SaveToFile(), false);
            SaveToClipboardCommand = new DelegateCommand(SaveToClipboard, false);

            DiscardChangesCommand = new DelegateCommand(async () =>
            {
                Reset();

                await Update();
            }, false);

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

        BitmapSource _originalBmp;

        public BitmapSource OriginalBitmap
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
            UnsavedChanges = true;

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
            UnsavedChanges = false;

            _brightness = _contrastThreshold = 0;
            _imageEffect = ImageEffect.None;

            Rotation = 0;
            FlipX = FlipY = false;

            InkCanvas.Strokes.Clear();

            _undoStack.Clear();
            _redoStack.Clear();
            UndoCommand.RaiseCanExecuteChanged(false);
            RedoCommand.RaiseCanExecuteChanged(false);
        }

        string _fileName;

        public void Open()
        {
            var ofd = new OpenFileDialog
            {
                Filter = "PNG Image|*.png",
                CheckFileExists = true,
                CheckPathExists = true
            };

            if (ofd.ShowDialog().GetValueOrDefault())
            {
                OpenFile(ofd.FileName);
            }
        }

        public void OpenFile(string FilePath)
        {
            _fileName = FilePath;

            var decoder = BitmapDecoder.Create(new Uri(FilePath),
                BitmapCreateOptions.None, BitmapCacheOption.OnLoad);

            Open(decoder.Frames[0]);
        }

        public void NewBlank()
        {
            var w = (int) InkCanvas.ActualWidth;
            var h = (int) InkCanvas.ActualHeight;

            var stride = w * 4;
            var data = new byte[h * stride];

            // white
            for (var i = 0; i < data.Length; ++i)
            {
                data[i] = 255;
            }

            Open(BitmapSource.Create(w, h, 96, 96, PixelFormats.Bgr32, null, data, stride));
        }

        public void OpenFromClipboard()
        {
            var img = ImageFromClipboard.Get();

            if (img == null)
            {
                ModernDialog.ShowMessage("No Image on Clipboard", "No Image on Clipboard", MessageBoxButton.OK, Window);

                return;
            }

            Open(img);
        }

        public void Open(BitmapSource Frame)
        {
            Reset();

            OriginalBitmap = Frame;

            _stride = OriginalBitmap.PixelWidth * (OriginalBitmap.Format.BitsPerPixel / 8);

            _data = new byte[_stride * OriginalBitmap.PixelHeight];

            EditedBitmap = new WriteableBitmap(OriginalBitmap);

            UpdateTransformBitmap();

            SaveCommand.RaiseCanExecuteChanged(true);
            SaveToClipboardCommand.RaiseCanExecuteChanged(true);
            DiscardChangesCommand.RaiseCanExecuteChanged(true);

            SetEffectCommand.RaiseCanExecuteChanged(true);
            SetBrightnessCommand.RaiseCanExecuteChanged(true);
            SetContrastCommand.RaiseCanExecuteChanged(true);

            RotateRightCommand.RaiseCanExecuteChanged(true);
            RotateLeftCommand.RaiseCanExecuteChanged(true);
            FlipXCommand.RaiseCanExecuteChanged(true);
            FlipYCommand.RaiseCanExecuteChanged(true);
        }

        public InkCanvas InkCanvas { get; set; }

        public void AddInkHistory(StrokeHistory HistoryItem)
        {
            if (!_trackChanges)
                return;

            UnsavedChanges = true;

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

        public bool SaveToFile()
        {
            var bmp = GetBmp();

            var encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(bmp));

            var sfd = new SaveFileDialog
            {
                Filter = "PNG Image|*.png",
                DefaultExt = ".png",
                AddExtension = true
            };

            if (_fileName != null)
            {
                var dir = Path.GetDirectoryName(_fileName);

                if (dir != null)
                    sfd.InitialDirectory = dir;
                
                sfd.FileName = Path.GetFileName(_fileName);
            }
            else sfd.FileName = "Untitled.png";

            if (sfd.ShowDialog().GetValueOrDefault())
            {
                using (var stream = sfd.OpenFile())
                    encoder.Save(stream);

                UnsavedChanges = false;

                return true;
            }

            return false;
        }

        void SaveToClipboard()
        {
            var bmp = GetBmp();

            Clipboard.SetImage(bmp);
        }

        BitmapSource GetBmp()
        {
            var drawingVisual = new DrawingVisual();

            var copy = EditedBitmap;
            var transform = TransformedBitmap.Transform;

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

                return transformedRendered;
            }
        }
    }
}