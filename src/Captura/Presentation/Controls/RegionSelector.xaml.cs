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
        public Rectangle Rectangle
        {
            get => Dispatcher.Invoke(() => new Rectangle((int) Left + 7, (int) Top + 37, (int) Width - 14, (int) Height - 44));
            set
            {
                Dispatcher.Invoke(() =>
                {
                    Left = value.Left - 7;
                    Top = value.Top - 37;

                    Width = value.Width + 14;
                    Height = value.Height + 44;
                });
            }
        }

        void HeaderPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e) => DragMove();

        void HeaderMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (WindowState == WindowState.Maximized)
                WindowState = WindowState.Normal;
        }
    }
}
