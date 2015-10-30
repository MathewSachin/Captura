using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System;

namespace Captura
{
    public partial class RegionSelector : Window
    {
        HwndSource RegSelhWnd;

        public RegionSelector() 
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
