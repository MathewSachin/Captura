namespace Captura.Views
{
    public partial class TranslationWindow
    {
        TranslationWindow()
        {
            InitializeComponent();
        }

        static TranslationWindow _instance;

        public static void ShowInstance()
        {
            if (_instance == null)
            {
                _instance = new TranslationWindow();

                _instance.Closed += (S, E) => _instance = null;
            }

            _instance.ShowAndFocus();
        }
    }
}
