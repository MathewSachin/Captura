using System.Windows;

namespace Captura
{
    public partial class FFmpegLogView
    {
        FFmpegLogView()
        {
            InitializeComponent();
        }

        void CloseButton_Click(object Sender, RoutedEventArgs E) => Close();

        static FFmpegLogView _instance;

        public static void ShowInstance()
        {
            if (_instance == null)
            {
                _instance = new FFmpegLogView();

                _instance.Closed += (S, E) => _instance = null;
            }

            _instance.ShowAndFocus();
        }
    }
}
