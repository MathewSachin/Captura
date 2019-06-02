using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using FirstFloor.ModernUI.Windows.Controls;
using Reactive.Bindings.Extensions;

namespace Captura
{
    public partial class ImageEditorWindow
    {
        public ImageEditorWindow()
        {
            InitializeComponent();

            if (DataContext is ImageEditorViewModel vm)
            {
                vm.ObserveProperty(M => M.TransformedBitmap)
                    .Subscribe(M => UpdateInkCanvas());

                vm.InkCanvas = InkCanvas;
                vm.Window = this;

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

                var inkCanvasVm = vm.InkCanvasViewModel;

                inkCanvasVm
                    .SelectedColor
                    .Subscribe(M => InkCanvas.DefaultDrawingAttributes.Color = M);

                inkCanvasVm
                    .BrushSize
                    .Subscribe(M => InkCanvas.DefaultDrawingAttributes.Height = InkCanvas.DefaultDrawingAttributes.Width = M);

                inkCanvasVm
                    .SelectedTool
                    .Subscribe(M => InkCanvas.SetInkTool(M));
            }

            Image.SizeChanged += (S, E) => UpdateInkCanvas();

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

        void Exit(object Sender, RoutedEventArgs E) => Close();

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

        void NewWindow(object Sender, RoutedEventArgs E) => new ImageEditorWindow().ShowAndFocus();

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
