namespace Captura
{
    public partial class SettingsWindow
    {
        SettingsWindow()
        {
            InitializeComponent();
        }

        static SettingsWindow _instance;

        public static void ShowInstance()
        {
            if (_instance == null)
            {
                _instance = new SettingsWindow();

                _instance.Closed += (S, E) => _instance = null;
            }

            _instance.ShowAndFocus();
        }
    }
}
