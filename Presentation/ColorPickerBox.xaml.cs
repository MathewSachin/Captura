using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Captura
{
    public partial class ColorPickerBox : UserControl
    {
        public ColorPickerBox()
        {
            InitializeComponent();
            DataContext = this;
        }

        public Brush SelectedColor
        {
            get { return (Brush)GetValue(SelectedColorProperty); }
            set { SetValue(SelectedColorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SelectedColor.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectedColorProperty =
            DependencyProperty.Register("SelectedColor", typeof(Brush), typeof(ColorPickerBox), new UIPropertyMetadata(null));
    }
}
