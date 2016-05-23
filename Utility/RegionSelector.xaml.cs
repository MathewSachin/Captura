using System;
using System.Drawing;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;

namespace Captura
{
    public partial class RegionSelector
    {
        readonly HwndSource _regSelhWnd;

        public static RegionSelector Instance { get; } = new RegionSelector();

        RegionSelector()
        {
            InitializeComponent();

            Show();
            _regSelhWnd = (HwndSource)PresentationSource.FromVisual(this);
            Hide();
        }

        public IntPtr Handle => _regSelhWnd.Handle;

        public Rectangle Rectangle => new Rectangle((int)Left, (int)Top, (int)Width, (int)Height);

        void HeaderPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e) => DragMove();

        void HeaderMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (WindowState == WindowState.Maximized)
                WindowState = WindowState.Normal;
        }
    }
}
