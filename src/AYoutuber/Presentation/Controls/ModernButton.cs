using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Captura
{
    public class ModernButton : Button
    {
        public static readonly DependencyProperty IconDataProperty = DependencyProperty.Register(nameof(IconData), typeof(Geometry), typeof(ModernButton));
     
        public ModernButton() { DefaultStyleKey = typeof(ModernButton); }
        
        public Geometry IconData
        {
            get { return (Geometry)GetValue(IconDataProperty); }
            set { SetValue(IconDataProperty, value); }
        }
    }
}
