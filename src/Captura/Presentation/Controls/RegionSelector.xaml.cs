using Captura.Models;
using System;
using System.Drawing;
using System.Windows;
using System.Windows.Interop;
using Captura.ViewModels;

namespace Captura
{
    public partial class RegionSelector : IRegionProvider
    {
        readonly IVideoSourcePicker _videoSourcePicker;

        public RegionSelector(IVideoSourcePicker VideoSourcePicker)
        {
            _videoSourcePicker = VideoSourcePicker;

            InitializeComponent();

            VideoSource = new RegionItem(this);

            // Prevent Closing by User
            Closing += (S, E) => E.Cancel = true;

            InitDimensionBoxes();

            // Setting MainViewModel as DataContext from XAML causes crash.
            Loaded += (S, E) => MainControls.DataContext = ServiceProvider.Get<MainViewModel>();
        }

        const int LeftOffset = 7,
            TopOffset = 37,
            WidthBorder = 14,
            HeightBorder = 74;

        Rectangle? _region;
        
        void InitDimensionBoxes()
        {
            WidthBox.Minimum = (int)((MinWidth - WidthBorder) * Dpi.X);
            HeightBox.Minimum = (int)((MinHeight - HeightBorder) * Dpi.Y);

            void SizeChange()
            {
                var selectedRegion = SelectedRegion;

                WidthBox.Value = selectedRegion.Width;
                HeightBox.Value = selectedRegion.Height;
            }

            SizeChanged += (S, E) => SizeChange();

            SizeChange();

            WidthBox.ValueChanged += (S, E) =>
            {
                if (E.NewValue is int width)
                {
                    var selectedRegion = SelectedRegion;

                    selectedRegion.Width = width;

                    SelectedRegion = selectedRegion;
                }
            };

            HeightBox.ValueChanged += (S, E) =>
            {
                if (E.NewValue is int height)
                {
                    var selectedRegion = SelectedRegion;

                    selectedRegion.Height = height;

                    SelectedRegion = selectedRegion;
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

        public IVideoItem VideoSource { get; }
        #endregion

        void Snapper_OnClick(object Sender, RoutedEventArgs E)
        {
            var wih = new WindowInteropHelper(this);

            var win = _videoSourcePicker.PickWindow(new [] { wih.Handle });

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
