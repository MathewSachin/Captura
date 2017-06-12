using FirstFloor.ModernUI.Presentation;
using System.Windows;
using System.Windows.Media;

namespace Captura
{
    public partial class MoreOptionsWindow
    {
        public MoreOptionsWindow()
        {
            InitializeComponent();
        }

        void SelectedAccentColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            if (e.NewValue != null)
            {
                AppearanceManager.Current.AccentColor = e.NewValue.Value;

                Settings.Instance.AccentColor = e.NewValue.Value.ToString();
            }
        }
    }
}
