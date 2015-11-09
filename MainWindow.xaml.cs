namespace Captura
{
    public partial class MainWindow : FirstFloor.ModernUI.Windows.Controls.ModernWindow//, INotifyPropertyChanged
    {
        public static MainWindow Instance { get; private set; }

        // ToDo
        // * ScreenShot BackgroundColor
        // * ScreenShot Resize Window
        
        public MainWindow()
        {
            InitializeComponent();

            Instance = this;
        }
    }
}