using System;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using FirstFloor.ModernUI.Windows.Controls;
using Microsoft.Win32;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;

namespace Captura
{
    public class ImageEditorViewModel : NotifyPropertyChanged
    {
        #region Fields
        const int BrightnessStep = 10;
        const int ContrastStep = 10;

        int _stride;
        byte[] _data;
        int _editingOperationCount;
        string _fileName;
        bool _trackChanges = true;

        BitmapSource _originalBmp;
        WriteableBitmap _editedBmp;
        TransformedBitmap _transformedBmp;

        readonly HistoryStack _undoStack = new HistoryStack();
        readonly HistoryStack _redoStack = new HistoryStack();

        readonly IReactiveProperty<bool> _unsavedChanges = new ReactivePropertySlim<bool>();
        readonly IReactiveProperty<bool> _isOpened = new ReactivePropertySlim<bool>();
        readonly IReactiveProperty<ImageEffect> _currentImageEffect = new ReactivePropertySlim<ImageEffect>();
        readonly IReactiveProperty<int> _brightness = new ReactivePropertySlim<int>();
        readonly IReactiveProperty<int> _contrastThreshold = new ReactivePropertySlim<int>();
        readonly IReadOnlyReactiveProperty<double> _contrastLevel;

        public IReactiveProperty<int> Rotation { get; } = new ReactivePropertySlim<int>();
        public IReactiveProperty<bool> FlipX { get; } = new ReactivePropertySlim<bool>();
        public IReactiveProperty<bool> FlipY { get; } = new ReactivePropertySlim<bool>();

        public Window Window { get; set; }
        public InkCanvas InkCanvas { get; set; }

        public bool UnsavedChanges
        {
            get => _unsavedChanges.Value;
            private set => _unsavedChanges.Value = value;
        }

        public BitmapSource OriginalBitmap
        {
            get => _originalBmp;
            private set => Set(ref _originalBmp, value);
        }

        public WriteableBitmap EditedBitmap
        {
            get => _editedBmp;
            private set => Set(ref _editedBmp, value);
        }

        public TransformedBitmap TransformedBitmap
        {
            get => _transformedBmp;
            set => Set(ref _transformedBmp, value);
        }
        #endregion

        #region Commands
        public ICommand DiscardChangesCommand { get; }
        public ICommand UndoCommand { get; }
        public ICommand RedoCommand { get; }
        public ICommand SaveCommand { get; }
        public ICommand SaveToClipboardCommand { get; }
        public ICommand UploadToImgurCommand { get; }

        public ICommand SetEffectCommand { get; }
        public ICommand SetBrightnessCommand { get; }
        public ICommand SetContrastCommand { get; }

        public ICommand RotateRightCommand { get; }
        public ICommand RotateLeftCommand { get; }
        public ICommand FlipXCommand { get; }
        public ICommand FlipYCommand { get; }
        #endregion

        public ImageEditorViewModel()
        {
            UndoCommand = _undoStack
                .ObserveProperty(M => M.Count)
                .Select(M => M > 0)
                .ToReactiveCommand()
                .WithSubscribe(Undo);

            RedoCommand = _redoStack
                .ObserveProperty(M => M.Count)
                .Select(M => M > 0)
                .ToReactiveCommand()
                .WithSubscribe(Redo);

            SaveCommand = _isOpened
                .ToReactiveCommand()
                .WithSubscribe(() => SaveToFile());

            SaveToClipboardCommand = _isOpened
                .ToReactiveCommand()
                .WithSubscribe(SaveToClipboard);

            UploadToImgurCommand = _isOpened
                .ToReactiveCommand()
                .WithSubscribe(UploadToImgur);

            DiscardChangesCommand = _unsavedChanges
                .ToReactiveCommand()
                .WithSubscribe(Reset);

            SetEffectCommand = _isOpened
                .ToReactiveCommand<ImageEffect>()
                .WithSubscribe(M =>
                {
                    UpdateHistory();

                    _currentImageEffect.Value = M;
                });

            SetBrightnessCommand = _isOpened
                .ToReactiveCommand()
                .WithSubscribe(M =>
                {
                    if (M is int i || M is string s && int.TryParse(s, out i))
                    {
                        UpdateHistory();

                        if (i == 0)
                            _brightness.Value = 0;
                        else if (i > 0)
                            _brightness.Value += BrightnessStep;
                        else _brightness.Value -= BrightnessStep;
                    }
                });

            SetContrastCommand = _isOpened
                .ToReactiveCommand()
                .WithSubscribe(M =>
                {
                    if (M is int i || M is string s && int.TryParse(s, out i))
                    {
                        UpdateHistory();

                        if (i == 0)
                            _contrastThreshold.Value = 0;
                        else if (i > 0)
                        {
                            if (_contrastThreshold.Value == 100)
                                return;

                            _contrastThreshold.Value += ContrastStep;
                        }
                        else
                        {
                            if (_contrastThreshold.Value == -100)
                                return;

                            _contrastThreshold.Value -= ContrastStep;
                        }
                    }
                });

            RotateRightCommand = _isOpened
                .ToReactiveCommand()
                .WithSubscribe(M =>
                {
                    UpdateHistory();

                    Rotation.Value += 90;
                });

            RotateLeftCommand = _isOpened
                .ToReactiveCommand()
                .WithSubscribe(M =>
                {
                    UpdateHistory();

                    Rotation.Value -= 90;
                });

            FlipXCommand = _isOpened
                .ToReactiveCommand()
                .WithSubscribe(M =>
                {
                    UpdateHistory();

                    FlipX.Value = !FlipX.Value;
                });

            FlipYCommand = _isOpened
                .ToReactiveCommand()
                .WithSubscribe(M =>
                {
                    UpdateHistory();

                    FlipY.Value = !FlipY.Value;
                });

            _contrastLevel = _contrastThreshold
                .Select(M => Math.Pow((100.0 + M) / 100.0, 2))
                .ToReadOnlyReactivePropertySlim();

            this.ObserveProperty(M => M.OriginalBitmap)
                .CombineLatest(
                    _brightness,
                    _contrastThreshold,
                    Rotation,
                    FlipX,
                    FlipY,
                    _currentImageEffect,
                    (Bmp, B, C, R, Fx, Fy, E) => Bmp)
                .Throttle(TimeSpan.FromMilliseconds(100))
                .Where(M => M != null)
                .ObserveOnUIDispatcher()
                .Subscribe(async M => await Update());
        }

        void Brightness(ref byte Red, ref byte Green, ref byte Blue)
        {
            void Apply(ref byte Byte)
            {
                var val = Byte + _brightness.Value;

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

        void Contrast(ref byte Red, ref byte Green, ref byte Blue)
        {
            void Apply(ref byte Byte)
            {
                var val = ((Byte / 255.0 - 0.5) * _contrastLevel.Value + 0.5) * 255.0;

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

        async Task Update()
        {
            OriginalBitmap.CopyPixels(_data, _stride, 0);

            var effectFunction = PixelFunctionFactory.GetEffectFunction(_currentImageEffect.Value);

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

        void UpdateTransformBitmap()
        {
            var rotate = new RotateTransform(Rotation.Value, OriginalBitmap.PixelWidth / 2.0, OriginalBitmap.PixelHeight / 2.0);
            var scale = new ScaleTransform(FlipX.Value ? -1 : 1, FlipY.Value ? -1 : 1);

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
            _brightness.Value = _contrastThreshold.Value = 0;
            _currentImageEffect.Value = ImageEffect.None;

            Rotation.Value = 0;
            FlipX.Value = FlipY.Value = false;

            InkCanvas.Strokes.Clear();

            _undoStack.Clear();
            _redoStack.Clear();

            UnsavedChanges = false;
        }

        #region New / Open
        public void Open()
        {
            var ofd = new OpenFileDialog
            {
                Filter = "Image Files|*.png;*.jpg;*.jpeg;*.bmp;*.wmp;*.tiff",
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

            _isOpened.Value = true;
        }
        #endregion

        #region History
        HistoryState GetHistoryState()
        {
            return new HistoryState
            {
                Brightness = _brightness.Value,
                Contrast = _contrastThreshold.Value,
                Effect = _currentImageEffect.Value,
                Rotation = Rotation.Value,
                FlipX = FlipX.Value,
                FlipY = FlipY.Value
            };
        }

        void UpdateHistory()
        {
            UnsavedChanges = true;

            _undoStack.Push(GetHistoryState());

            _redoStack.Clear();
        }

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
            }

            _redoStack.Clear();
        }

        public void AddSelectHistory(SelectHistory HistoryItem)
        {
            if (!_trackChanges)
                return;

            UnsavedChanges = true;

            HistoryItem.EditingOperationCount = _editingOperationCount;

            var merged = false;

            if (_undoStack.Count > 0)
            {
                var peek = _undoStack.Peek();

                if (peek is SelectHistory select
                    && HistoryItem.EditingOperationCount == select.EditingOperationCount
                    && StrokeCollectionsAreEqual(HistoryItem.Selection, select.Selection))
                {
                    select.NewRect = HistoryItem.NewRect;

                    merged = true;
                }
            }

            if (!merged)
            {
                _undoStack.Push(HistoryItem);
            }

            _redoStack.Clear();
        }

        public void RemoveLastHistory()
        {
            _trackChanges = false;

            if (_undoStack.Count == 0)
                return;

            _undoStack.Pop();

            _trackChanges = true;
        }

        void Undo()
        {
            _trackChanges = false;

            if (_undoStack.Count == 0)
                return;

            var current = GetHistoryState();

            var item = _undoStack.Pop();

            switch (item)
            {
                case HistoryState state:
                    ApplyHistoryState(state);

                    _redoStack.Push(current);
                    break;

                case StrokeHistory strokes:
                    foreach (var stroke in strokes.Added)
                    {
                        InkCanvas.Strokes.Remove(stroke);
                    }

                    foreach (var stroke in strokes.Removed)
                    {
                        InkCanvas.Strokes.Add(stroke);
                    }

                    _redoStack.Push(item);
                    break;

                case SelectHistory select:
                    var m = GetTransformFromRectToRect(select.NewRect, select.OldRect);
                    select.Selection.Transform(m, false);

                    _redoStack.Push(select);
                    break;
            }

            _trackChanges = true;
        }

        void Redo()
        {
            _trackChanges = false;

            if (_redoStack.Count == 0)
                return;

            var current = GetHistoryState();

            var item = _redoStack.Pop();

            switch (item)
            {
                case HistoryState state:
                    ApplyHistoryState(state);

                    _undoStack.Push(current);
                    break;

                case StrokeHistory strokes:
                    foreach (var stroke in strokes.Added)
                    {
                        InkCanvas.Strokes.Add(stroke);
                    }

                    foreach (var stroke in strokes.Removed)
                    {
                        InkCanvas.Strokes.Remove(stroke);
                    }

                    _undoStack.Push(item);
                    break;

                case SelectHistory select:
                    var m = GetTransformFromRectToRect(select.OldRect, select.NewRect);
                    select.Selection.Transform(m, false);

                    _undoStack.Push(select);
                    break;
            }

            _trackChanges = true;
        }

        void ApplyHistoryState(HistoryState State)
        {
            _currentImageEffect.Value = State.Effect;
            _brightness.Value = State.Brightness;
            _contrastThreshold.Value = State.Contrast;
            Rotation.Value = State.Rotation;
            FlipX.Value = State.FlipX;
            FlipY.Value = State.FlipY;
        }
        #endregion

        #region Save
        public bool SaveToFile()
        {
            var bmp = GetBmp();

            if (!bmp.SaveToPickedFile(_fileName))
                return false;

            UnsavedChanges = false;

            return true;
        }

        void SaveToClipboard()
        {
            var bmp = GetBmp();

            Clipboard.SetImage(bmp);
        }

        async void UploadToImgur()
        {
            using (var ms = new MemoryStream())
            {
                var bmp = GetBmp();

                var encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(bmp));

                encoder.Save(ms);

                var imgSystem = ServiceProvider.Get<IImagingSystem>();

                using (var bitmap = imgSystem.LoadBitmap(ms))
                {
                    await bitmap.UploadImage();
                }
            }
        }

        BitmapSource GetBmp()
        {
            var drawingVisual = new DrawingVisual();

            var copy = EditedBitmap;
            var transform = TransformedBitmap.Transform;

            using (var drawingContext = drawingVisual.RenderOpen())
            {
                drawingContext.DrawImage(copy, new Rect(0, 0, copy.Width, copy.Height));

                var strokesCopy = InkCanvas.Strokes.Clone();

                var matrix = Matrix.Identity;
                matrix.Scale(96 / copy.DpiX, 96 / copy.DpiY);

                strokesCopy.Transform(matrix, true);

                strokesCopy.Draw(drawingContext);

                drawingContext.Close();

                var bitmap = new RenderTargetBitmap(copy.PixelWidth,
                    copy.PixelHeight,
                    copy.DpiX,
                    copy.DpiY,
                    PixelFormats.Pbgra32);

                bitmap.Render(drawingVisual);

                var transformedRendered = new TransformedBitmap(bitmap, transform);

                return transformedRendered;
            }
        }
        #endregion

        public void IncrementEditingOperationCount()
        {
            ++_editingOperationCount;
        }

        static Matrix GetTransformFromRectToRect(Rect Source, Rect Destination)
        {
            var m = Matrix.Identity;

            m.Translate(-Source.X, -Source.Y);
            m.Scale(Destination.Width / Source.Width, Destination.Height / Source.Height);
            m.Translate(Destination.X, Destination.Y);

            return m;
        }

        static bool StrokeCollectionsAreEqual(StrokeCollection A, StrokeCollection B)
        {
            if (A == null && B == null)
                return true;

            if (A == null || B == null)
                return false;

            if (A.Count != B.Count)
                return false;

            return !A.Where((T, I) => T != B[I]).Any();
        }
    }
}