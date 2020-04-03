using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using Captura.ViewModels;
using Color = System.Windows.Media.Color;
using Cursors = System.Windows.Input.Cursors;

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

            ViewModel
                .BrushColor
                .Subscribe(M => InkCanvas.DefaultDrawingAttributes.Color = M);

            ViewModel
                .BrushSize
                .Subscribe(M => InkCanvas.DefaultDrawingAttributes.Height = InkCanvas.DefaultDrawingAttributes.Width = M);

            ViewModel
                .SelectedTool
                .Subscribe(OnToolChange);

            ViewModel
                .ClearAllDrawingsCommand
                .Subscribe(() => InkCanvas.Strokes.Clear());

            InkCanvas.DefaultDrawingAttributes.FitToCurve = true;
        }

        void OnToolChange(InkCanvasEditingMode Tool)
        {
            InkCanvas.EditingMode = Tool;

            if (Tool == InkCanvasEditingMode.Ink)
            {
                InkCanvas.UseCustomCursor = true;
                InkCanvas.Cursor = Cursors.Pen;
            }
            else InkCanvas.UseCustomCursor = false;

            InkCanvas.Background = new SolidColorBrush(Tool == InkCanvasEditingMode.None
                ? Colors.Transparent
                : Color.FromArgb(1, 0, 0, 0));
        }

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
    }
}
