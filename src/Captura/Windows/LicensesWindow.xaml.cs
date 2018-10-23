namespace Captura.Views
{
    public partial class LicensesWindow
    {
        LicensesWindow()
        {
            InitializeComponent();
        }

        static LicensesWindow _instance;

        public static void ShowInstance()
        {
            if (_instance == null)
            {
                _instance = new LicensesWindow();

                _instance.Closed += (S, E) => _instance = null;
            }

            _instance.ShowAndFocus();
        }
    }
}
