namespace Captura.Views
{
    public partial class HotkeysWindow
    {
        HotkeysWindow()
        {
            InitializeComponent();
        }

        static HotkeysWindow _instance;

        public static void ShowInstance()
        {
            if (_instance == null)
            {
                _instance = new HotkeysWindow();

                _instance.Closed += (S, E) => _instance = null;
            }

            _instance.ShowAndFocus();
        }
    }
}
