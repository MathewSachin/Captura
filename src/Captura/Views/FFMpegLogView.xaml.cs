using System.Windows;

namespace Captura
{
    public partial class FFmpegLogView
    {
        FFmpegLogView()
        {
            InitializeComponent();

            Closing += (S, E) =>
            {
                Hide();

                E.Cancel = true;
            };
        }

        public static FFmpegLogView Instance { get; } = new FFmpegLogView();

        void CloseButton_Click(object Sender, RoutedEventArgs E) => Close();
    }
}
