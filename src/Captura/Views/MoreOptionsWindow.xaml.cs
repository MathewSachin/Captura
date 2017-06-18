using FirstFloor.ModernUI.Presentation;
using System.Windows;
using System.Windows.Media;

namespace Captura
{
    public partial class MoreOptionsWindow
    {
        MoreOptionsWindow()
        {
            InitializeComponent();

            Closing += (s, e) =>
            {
                Hide();

                e.Cancel = true;
            };
        }

        public static MoreOptionsWindow Instance { get; } = new MoreOptionsWindow();

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
