namespace Captura
{
    public partial class MainWindow : FirstFloor.ModernUI.Windows.Controls.ModernWindow
    {
        public static MainWindow Instance { get; private set; }
        
        public MainWindow()
        {
            InitializeComponent();

            Instance = this;
        }
    }
}