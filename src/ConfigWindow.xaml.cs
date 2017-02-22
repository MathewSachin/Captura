namespace Captura
{
    public partial class ConfigWindow
    {
        static ConfigWindow _instance;
        
        ConfigWindow()
        {
            InitializeComponent();

            Closing += (s, e) =>
            {
                Hide();
                e.Cancel = true;
            };
        }

        public static void ShowFocused()
        {
            if (_instance == null)
                _instance = new ConfigWindow();

            _instance.Show();
            _instance.Focus();
        }
    }
}
