using FirstFloor.ModernUI.Presentation;
using System.Windows;
using System.Windows.Media;
using Captura.ViewModels;

namespace Captura
{
    public partial class ExtrasPage
    {
        void SelectedAccentColorChanged(object Sender, RoutedPropertyChangedEventArgs<Color?> E)
        {
            if (E.NewValue != null && DataContext is ViewModelBase vm)
            {
                AppearanceManager.Current.AccentColor = E.NewValue.Value;

                vm.Settings.UI.AccentColor = E.NewValue.Value.ToString();
            }
        }

        void DarkThemeClick(object Sender, RoutedEventArgs E)
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
