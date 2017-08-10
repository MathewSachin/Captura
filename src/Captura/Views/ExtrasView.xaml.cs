using FirstFloor.ModernUI.Presentation;
using System.Windows;
using System.Windows.Media;

namespace Captura
{
    public partial class ExtrasView
    {
        void SelectedAccentColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            if (e.NewValue != null)
            {
                AppearanceManager.Current.AccentColor = e.NewValue.Value;

                Settings.Instance.AccentColor = e.NewValue.Value.ToString();
            }
        }

        void DarkThemeClick(object sender, RoutedEventArgs e)
        {
            AppearanceManager.Current.ThemeSource = Settings.Instance.DarkTheme
                ? AppearanceManager.DarkThemeSource : AppearanceManager.LightThemeSource;
        }
    }
}
