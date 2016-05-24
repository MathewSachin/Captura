using System.Windows;
using System.Windows.Media;

namespace Captura
{
    public partial class ColorPickerBox
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

        public static readonly DependencyProperty SelectedColorProperty =
            DependencyProperty.Register("SelectedColor", typeof(Brush), typeof(ColorPickerBox), new UIPropertyMetadata(Brushes.Transparent));
    }
}
