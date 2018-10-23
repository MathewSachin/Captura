using System.Windows;

namespace Captura
{
    public partial class FFmpegLogWindow
    {
        FFmpegLogWindow()
        {
            InitializeComponent();
        }

        void CloseButton_Click(object Sender, RoutedEventArgs E) => Close();

        static FFmpegLogWindow _instance;

        public static void ShowInstance()
        {
            if (_instance == null)
            {
                _instance = new FFmpegLogWindow();

                _instance.Closed += (S, E) => _instance = null;
            }

            _instance.ShowAndFocus();
        }
    }
}
