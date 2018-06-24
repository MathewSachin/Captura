namespace Captura.Views
{
    public partial class CrashLogsWindow
    {
        CrashLogsWindow()
        {
            InitializeComponent();
        }

        static CrashLogsWindow _instance;

        public static void ShowInstance()
        {
            if (_instance == null)
            {
                _instance = new CrashLogsWindow();

                _instance.Closed += (S, E) => _instance = null;
            }

            _instance.ShowAndFocus();
        }
    }
}
