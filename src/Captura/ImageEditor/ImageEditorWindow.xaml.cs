using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using FirstFloor.ModernUI.Windows.Controls;

namespace Captura
{
    public partial class ImageEditorWindow
    {
        readonly ImageEditorSettings _settings;

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

                void SelectionMovingOrResizing(object Sender, InkCanvasSelectionEditingEventArgs Args)
                {
                    vm.AddSelectHistory(new SelectHistory
                    {
                        NewRect = Args.NewRectangle,
                        OldRect = Args.OldRectangle,
                        Selection = InkCanvas.GetSelectedStrokes()
                    });
                }

                InkCanvas.SelectionMoving += SelectionMovingOrResizing;
                InkCanvas.SelectionResizing += SelectionMovingOrResizing;

                vm.Window = this;
            }

            Image.SizeChanged += (S, E) => UpdateInkCanvas();

            ModesBox.SelectedIndex = 0;

            _settings = ServiceProvider.Get<Settings>().ImageEditor;

            ColorPicker.SelectedColor = _settings.BrushColor.ToWpfColor();
            SizeBox.Value = _settings.BrushSize;

            InkCanvas.DefaultDrawingAttributes.FitToCurve = true;
        }

        public void Open(string FilePath)
        {
            if (DataContext is ImageEditorViewModel vm)
            {
                vm.OpenFile(FilePath);
            }
        }

        public void Open(BitmapSource Bmp)
        {
            if (DataContext is ImageEditorViewModel vm)
            {
                vm.Open(Bmp);
            }
        }

        void Exit(object Sender, RoutedEventArgs E)
        {
            Close();
        }

        void SizeBox_OnValueChanged(object Sender, RoutedPropertyChangedEventArgs<object> E)
        {
            if (InkCanvas != null && E.NewValue is int i)
            {
                InkCanvas.DefaultDrawingAttributes.Height = InkCanvas.DefaultDrawingAttributes.Width = i;

                _settings.BrushSize = i;
            }
        }

        void ModesBox_OnSelectionChanged(object Sender, SelectionChangedEventArgs E)
        {
            if (ModesBox.SelectedValue is ExtendedInkTool tool)
            {
                InkCanvas.SetInkTool(tool);
            }
        }

        void ColorPicker_OnSelectedColorChanged(object Sender, RoutedPropertyChangedEventArgs<Color?> E)
        {
            if (E.NewValue != null && InkCanvas != null)
            {
                var color = E.NewValue.Value;

                InkCanvas.DefaultDrawingAttributes.Color = color;

                _settings.BrushColor = color.ToDrawingColor();
            }
        }

        void UpdateInkCanvas()
        {
            if (DataContext is ImageEditorViewModel vm && vm.TransformedBitmap != null && Image.ActualWidth > 0)
            {
                InkCanvas.IsEnabled = true;

                InkCanvas.Width = vm.OriginalBitmap.PixelWidth;
                InkCanvas.Height = vm.OriginalBitmap.PixelHeight;

                var rotate = new RotateTransform(vm.Rotation.Value,
                    vm.OriginalBitmap.PixelWidth / 2.0,
                    vm.OriginalBitmap.PixelHeight / 2.0);

                var tilted = Math.Abs(vm.Rotation.Value / 90) % 2 == 1;
                
                var scale = new ScaleTransform(
                    ((tilted ? Image.ActualHeight : Image.ActualWidth) / InkCanvas.Width) * (vm.FlipX.Value ? -1 : 1),
                    ((tilted ? Image.ActualWidth : Image.ActualHeight) / InkCanvas.Height) * (vm.FlipY.Value ? -1 : 1)
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

        void InkCanvas_OnMouseUp(object Sender, MouseButtonEventArgs E)
        {
            if (DataContext is ImageEditorViewModel vm)
            {
                vm.IncrementEditingOperationCount();
            }
        }

        void ImageEditorWindow_OnClosing(object Sender, CancelEventArgs E)
        {
            if (DataContext is ImageEditorViewModel vm)
            {
                if (vm.UnsavedChanges)
                {
                    var result = ModernDialog.ShowMessage("Do you want to save your changes before exiting?",
                        "Unsaved Changes",
                        MessageBoxButton.YesNoCancel,
                        this);

                    switch (result)
                    {
                        case MessageBoxResult.Yes when !vm.SaveToFile():
                        case MessageBoxResult.Cancel:
                            E.Cancel = true;
                            break;
                    }
                }
            }
        }

        void NewWindow(object Sender, RoutedEventArgs E)
        {
            new ImageEditorWindow().ShowAndFocus();
        }

        // Return false to cancel
        bool ConfirmSaveBeforeNew(ImageEditorViewModel ViewModel)
        {
            if (ViewModel.UnsavedChanges)
            {
                var result = ModernDialog.ShowMessage("Do you want to save your changes?",
                    "Unsaved Changes",
                    MessageBoxButton.YesNoCancel,
                    this);

                switch (result)
                {
                    case MessageBoxResult.Yes:
                        ViewModel.SaveCommand.ExecuteIfCan();
                        break;

                    case MessageBoxResult.Cancel:
                        return false;
                }
            }

            return true;
        }

        void NewBlank(object Sender, ExecutedRoutedEventArgs E)
        {
            if (DataContext is ImageEditorViewModel vm)
            {
                if (!ConfirmSaveBeforeNew(vm))
                    return;

                vm.NewBlank();
            }
        }

        void Open(object Sender, ExecutedRoutedEventArgs E)
        {
            if (DataContext is ImageEditorViewModel vm)
            {
                if (!ConfirmSaveBeforeNew(vm))
                    return;

                vm.Open();
            }
        }

        void OpenFromClipboard(object Sender, RoutedEventArgs E)
        {
            if (DataContext is ImageEditorViewModel vm)
            {
                if (!ConfirmSaveBeforeNew(vm))
                    return;

                vm.OpenFromClipboard();
            }
        }
    }
}
