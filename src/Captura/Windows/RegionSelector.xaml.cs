using Captura.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using Color = System.Windows.Media.Color;

namespace Captura
{
    public partial class RegionSelector
    {
        readonly RegionSelectorViewModel _viewModel;

        public RegionSelector(RegionSelectorViewModel ViewModel)
        {
            _viewModel = ViewModel;

            InitializeComponent();

            // Prevent Closing by User
            Closing += (S, E) => E.Cancel = true;

            ModesBox.ItemsSource = new[]
            {
                new KeyValuePair<InkCanvasEditingMode, string>(InkCanvasEditingMode.None, "Pointer"),
                new KeyValuePair<InkCanvasEditingMode, string>(InkCanvasEditingMode.Ink, "Pencil"),
                new KeyValuePair<InkCanvasEditingMode, string>(InkCanvasEditingMode.EraseByPoint, "Eraser"),
                new KeyValuePair<InkCanvasEditingMode, string>(InkCanvasEditingMode.EraseByStroke, "Stroke Eraser")
            };

            ModesBox.SelectedIndex = 0;
            ColorPicker.SelectedColor = Color.FromRgb(27, 27, 27);
            SizeBox.Value = 10;

            InkCanvas.DefaultDrawingAttributes.FitToCurve = true;
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

                InkCanvas.Background = new SolidColorBrush(mode == InkCanvasEditingMode.None
                    ? Colors.Transparent
                    : Color.FromArgb(1, 0, 0, 0));
            }
        }

        void ColorPicker_OnSelectedColorChanged(object Sender, RoutedPropertyChangedEventArgs<Color?> E)
        {
            if (E.NewValue != null && InkCanvas != null)
                InkCanvas.DefaultDrawingAttributes.Color = E.NewValue.Value;
        }

        void CloseButton_Click(object Sender, RoutedEventArgs E)
        {
            Hide();

            SelectorHidden?.Invoke();
        }

        public event Action SelectorHidden;

        // Prevent Maximizing
        protected override void OnStateChanged(EventArgs E)
        {
            if (WindowState != WindowState.Normal)
                WindowState = WindowState.Normal;

            base.OnStateChanged(E);
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo SizeInfo)
        {
            InkCanvas.Strokes.Clear();

            base.OnRenderSizeChanged(SizeInfo);
        }

        public IntPtr Handle => new WindowInteropHelper(this).Handle;

        void UIElement_OnPreviewMouseLeftButtonDown(object Sender, MouseButtonEventArgs E)
        {
            DragMove();
        }

        void Thumb_OnDragDelta(object Sender, DragDeltaEventArgs E)
        {
            void DoTop() => _viewModel.ResizeFromTop(E.VerticalChange);

            void DoLeft() => _viewModel.ResizeFromLeft(E.HorizontalChange);

            void DoBottom()
            {
                var height = Region.Height + E.VerticalChange;

                if (height > 0)
                    Region.Height = height;
            }

            void DoRight()
            {
                var width = Region.Width + E.HorizontalChange;

                if (width > 0)
                    Region.Width = width;
            }

            if (Sender is FrameworkElement element)
            {
                switch (element.Tag)
                {
                    case "Bottom":
                        DoBottom();
                        break;

                    case "Left":
                        DoLeft();
                        break;

                    case "Right":
                        DoRight();
                        break;

                    case "TopLeft":
                        DoTop();
                        DoLeft();
                        break;

                    case "TopRight":
                        DoTop();
                        DoRight();
                        break;

                    case "BottomLeft":
                        DoBottom();
                        DoLeft();
                        break;

                    case "BottomRight":
                        DoBottom();
                        DoRight();
                        break;
                }
            }
        }

        async void Snap_Click(object sender, RoutedEventArgs e)
        {
            Visibility = Visibility.Collapsed;

            // Ensure region selector hides
            await Task.Delay(1);

            _viewModel.SnapToRegion();

            Visibility = Visibility.Visible;
        }
    }
}
