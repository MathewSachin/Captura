using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Captura
{
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
