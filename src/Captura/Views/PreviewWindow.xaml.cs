using System.Windows.Controls;
using System.Windows.Media;

namespace Captura
{
    public partial class PreviewWindow
    {
        public PreviewWindow()
        {
            InitializeComponent();

            StrectValues.ItemsSource = new[]
            {
                Stretch.Uniform,
                Stretch.Fill,
                Stretch.UniformToFill
            };

            StrectValues.SelectedIndex = 0;
        }

        void StrectValues_OnSelectionChanged(object Sender, SelectionChangedEventArgs E)
        {
            if (StrectValues.SelectedValue is Stretch stretch)
            {
                DisplayImage.Stretch = stretch;
            }
        }
    }
}
