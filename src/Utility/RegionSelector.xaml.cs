using System.Drawing;
using System.Windows;
using System.Windows.Input;
using Window = Screna.Window;

namespace Captura
{
    public partial class RegionSelector
    {
        public static RegionSelector Instance { get; } = new RegionSelector();

        RegionSelector()
        {
            InitializeComponent();
        }

        public Window Window { get; }

        // Ignoring Borders
        public Rectangle Rectangle => Dispatcher.Invoke(() => new Rectangle((int)Left + 3, (int)Top + 3, (int)Width - 6, (int)Height - 6));

        void HeaderPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e) => DragMove();

        void HeaderMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (WindowState == WindowState.Maximized)
                WindowState = WindowState.Normal;
        }
    }
}
