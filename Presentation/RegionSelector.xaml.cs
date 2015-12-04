using System;
using System.Drawing;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;

namespace Captura
{
    public partial class RegionSelector : Window
    {
        HwndSource RegSelhWnd;

        public static RegionSelector Instance { get; private set; }

        static RegionSelector() { Instance = new RegionSelector(); }

        RegionSelector()
        {
            InitializeComponent();

            Show();
            RegSelhWnd = (HwndSource)HwndSource.FromVisual(this);
            Hide();
        }

        public IntPtr Handle { get { return RegSelhWnd.Handle; } }

        public Rectangle Rectangle { get { return new Rectangle((int)Left, (int)Top, (int)Width, (int)Height); } }

        void HeaderPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e) { DragMove(); }

        void HeaderMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (WindowState == WindowState.Maximized)
                WindowState = WindowState.Normal;
        }
    }
}
