using System.Drawing;
using System.Windows;
using System.Windows.Input;

namespace Captura
{
    public partial class RegionSelector
    {
        public static RegionSelector Instance { get; } = new RegionSelector();

        RegionSelector()
        {
            InitializeComponent();

            // Prevent Closing by User
            Closing += (s, e) => e.Cancel = true;
        }
        
        // Ignoring Borders and Header
        public Rectangle Rectangle => Dispatcher.Invoke(() => new Rectangle((int)Left + 3, (int)Top + 23, (int)Width - 6, (int)Height - 26));

        void HeaderPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e) => DragMove();

        void HeaderMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (WindowState == WindowState.Maximized)
                WindowState = WindowState.Normal;
        }
    }
}
