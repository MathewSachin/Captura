using FirstFloor.ModernUI.Presentation;
using System.Windows;
using System.Windows.Media;
using Captura.ViewModels;

namespace Captura
{
    public partial class ExtrasView
    {
        void SelectedAccentColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            if (e.NewValue != null && DataContext is ViewModelBase vm)
            {
                AppearanceManager.Current.AccentColor = e.NewValue.Value;

                vm.Settings.UI.AccentColor = e.NewValue.Value.ToString();
            }
        }

        void DarkThemeClick(object sender, RoutedEventArgs e)
        {
            if (DataContext is ViewModelBase vm)
            {
                AppearanceManager.Current.ThemeSource = vm.Settings.UI.DarkTheme
                    ? AppearanceManager.DarkThemeSource
                    : AppearanceManager.LightThemeSource;
            }
        }
    }
}
