using System.Windows;

namespace Captura
{
    public partial class FFMpegLogView
    {
        FFMpegLogView()
        {
            InitializeComponent();

            Closing += (S, E) =>
            {
                Hide();

                E.Cancel = true;
            };
        }

        public static FFMpegLogView Instance { get; } = new FFMpegLogView();

        void CloseButton_Click(object Sender, RoutedEventArgs E) => Close();
    }
}
