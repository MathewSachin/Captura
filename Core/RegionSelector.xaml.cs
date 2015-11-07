using System;
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

        void DockPanel_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e) { DragMove(); }
    }
}
