using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Captura
{
    public class ModernToggleButton : CheckBox
    {
        public static readonly DependencyProperty IconDataProperty = DependencyProperty.Register(nameof(IconData), typeof(Geometry), typeof(ModernToggleButton));
        
        public ModernToggleButton() { DefaultStyleKey = typeof(ModernToggleButton); }
        
        public Geometry IconData
        {
            get => (Geometry)GetValue(IconDataProperty);
            set => SetValue(IconDataProperty, value);
        }
    }
}
