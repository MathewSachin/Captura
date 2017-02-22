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

        public static void ShowInstance()
        {
            Ensure();

            _instance.Show();
            _instance.Focus();
        }

        static void Ensure()
        {
            if (_instance == null)
                _instance = new ConfigWindow();
        }

        public static void HideInstance()
        {
            Ensure();

            _instance.Hide();
        }
    }
}
