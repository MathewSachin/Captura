using System.Windows;

namespace Captura
{
    public partial class FontSelector
    {
        public FontSelector()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty SelectedFontProperty = DependencyProperty.Register(
            nameof(SelectedFont),
            typeof(string),
            typeof(FontSelector),
            new FrameworkPropertyMetadata("Arial"));

        public string SelectedFont
        {
            get => (string) GetValue(SelectedFontProperty);
            set => SetValue(SelectedFontProperty, value);
        }
    }
}
