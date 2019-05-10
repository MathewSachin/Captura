using MaterialDesignThemes.Wpf;

namespace Captura.View
{
    public class SettingsTab
    {
        public PackIconKind Icon { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }
    }

    public partial class SettingsWindow
    {
        public SettingsWindow()
        {
            InitializeComponent();
        }

        public SettingsTab[] SettingsTabs { get; } =
        {
            new SettingsTab
            {
                Icon = PackIconKind.VideoOutline,
                Title = "Video",
                Description = "Output format"
            },
            new SettingsTab
            {
                Icon = PackIconKind.Audio,
                Title = "Audio",
                Description = "Output type"
            }
        };
    }
}
