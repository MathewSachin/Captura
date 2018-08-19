using Captura.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using Captura.ViewModels;
using Color = System.Windows.Media.Color;

namespace Captura
{
    public partial class RegionSelector : IRegionProvider
    {
        readonly IVideoSourcePicker _videoSourcePicker;
        readonly RegionItem _regionItem;

        bool _widthBoxChanging, _heightBoxChanging, _resizing;

        public RegionSelector(IVideoSourcePicker VideoSourcePicker, LanguageManager Loc)
        {
            _videoSourcePicker = VideoSourcePicker;

            InitializeComponent();

            _regionItem = new RegionItem(this);

            // Prevent Closing by User
            Closing += (S, E) => E.Cancel = true;

            InitDimensionBoxes();

            // Setting MainViewModel as DataContext from XAML causes crash.
            Loaded += (S, E) => MainControls.DataContext = ServiceProvider.Get<MainViewModel>();

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

        const int LeftOffset = 43,
            TopOffset = 38,
            WidthBorder = LeftOffset + 7,
            HeightBorder = TopOffset + 37;

        Rectangle? _region;
        
        void InitDimensionBoxes()
        {
            WidthBox.Minimum = (int)((MinWidth - WidthBorder) * Dpi.X);
            HeightBox.Minimum = (int)((MinHeight - HeightBorder) * Dpi.Y);

            void SizeChange()
            {
                if (_widthBoxChanging || _heightBoxChanging)
                    return;

                _resizing = true;

                var selectedRegion = SelectedRegion;

                WidthBox.Value = selectedRegion.Width;
                HeightBox.Value = selectedRegion.Height;

                _resizing = false;
            }

            SizeChanged += (S, E) => SizeChange();

            SizeChange();

            WidthBox.ValueChanged += (S, E) =>
            {
                if (!_resizing && E.NewValue is int width)
                {
                    _widthBoxChanging = true;

                    var selectedRegion = SelectedRegion;

                    selectedRegion.Width = width;

                    SelectedRegion = selectedRegion;

                    _widthBoxChanging = false;
                }
            };

            HeightBox.ValueChanged += (S, E) =>
            {
                if (!_resizing && E.NewValue is int height)
                {
                    _heightBoxChanging = true;

                    var selectedRegion = SelectedRegion;

                    selectedRegion.Height = height;

                    SelectedRegion = selectedRegion;

                    _heightBoxChanging = false;
                }
            };
        }

        void CloseButton_Click(object Sender, RoutedEventArgs E)
        {
            Hide();

            SelectorHidden?.Invoke();
        }
        
        protected override void OnLocationChanged(EventArgs E)
        {
            base.OnLocationChanged(E);

            UpdateRegion();
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
            UpdateRegion();

            InkCanvas.Strokes.Clear();

            base.OnRenderSizeChanged(SizeInfo);
        }

        #region IRegionProvider
        public event Action SelectorHidden;

        public bool SelectorVisible
        {
            get => Visibility == Visibility.Visible;
            set
            {
                if (value)
                    Show();
                else Hide();
            }
        }
        
        void UpdateRegion()
        {
            _region = Dispatcher.Invoke(() =>
                new Rectangle((int)((Left + LeftOffset) * Dpi.X),
                    (int)((Top + TopOffset) * Dpi.Y),
                    (int)((Width - WidthBorder) * Dpi.X),
                    (int)((Height - HeightBorder) * Dpi.Y)));

            _regionItem.Name = _region.ToString().Replace("{", "")
                .Replace("}", "")
                .Replace(",", ", ");
        }

        // Ignoring Borders and Header
        public Rectangle SelectedRegion
        {
            get
            {
                if (_region == null)
                    UpdateRegion();

                return _region.Value;
            }
            set
            {
                if (value == Rectangle.Empty)
                    return;
                
                Dispatcher.Invoke(() =>
                {
                    Width = value.Width / Dpi.X + WidthBorder;
                    Height = value.Height / Dpi.Y + HeightBorder;

                    Left = value.Left / Dpi.X - LeftOffset;
                    Top = value.Top / Dpi.Y - TopOffset;
                });
            }
        }

        public void Lock()
        {
            Dispatcher.Invoke(() =>
            {
                ResizeMode = ResizeMode.NoResize;
                Snapper.IsEnabled = CloseButton.IsEnabled = false;

                WidthBox.IsEnabled = HeightBox.IsEnabled = false;

                if (ServiceProvider.Get<Settings>().UI.HideRegionSelectorWhenRecording)
                {
                    Hide();
                }
            });
        }
        
        public void Release()
        {
            Dispatcher.Invoke(() =>
            {
                ResizeMode = ResizeMode.CanResize;
                Snapper.IsEnabled = CloseButton.IsEnabled = true;

                WidthBox.IsEnabled = HeightBox.IsEnabled = true;

                Show();
            });
        }

        public IVideoItem VideoSource => _regionItem;

        public IntPtr Handle => new WindowInteropHelper(this).Handle;

        #endregion

        void Snapper_OnClick(object Sender, RoutedEventArgs E)
        {
            var win = _videoSourcePicker.PickWindow(new [] { Handle });

            if (win == null)
                return;

            SelectedRegion = win.Rectangle;

            // Prevent going outside
            if (Left < 0)
            {
                // Decrease Width
                try { Width += Left; }
                catch { }
                finally { Left = 0; }
            }

            if (Top < 0)
            {
                // Decrease Height
                try { Height += Top; }
                catch { }
                finally { Top = 0; }
            }
        }
    }
}
