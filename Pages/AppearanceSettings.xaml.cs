using System.Windows.Controls;

namespace Captura
{
    /// <summary>
    /// Interaction logic for AppearanceSettings.xaml
    /// </summary>
    public partial class AppearanceSettings : UserControl
    {
        public AppearanceSettings()
        {
            InitializeComponent();

            // a simple view model for appearance configuration
            this.DataContext = new AppearanceSettingsViewModel();
        }
    }
}
