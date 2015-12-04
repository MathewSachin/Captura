using System.ComponentModel;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Media;
using FirstFloor.ModernUI.Presentation;
using FontSizeEnum = FirstFloor.ModernUI.Presentation.FontSize;

namespace Captura
{
    public partial class AppearanceSettings : UserControl, INotifyPropertyChanged
    {
        public static readonly Link DarkTheme = new Link { DisplayName = "Dark", Source = AppearanceManager.DarkThemeSource },
            LightTheme = new Link { DisplayName = "Light", Source = AppearanceManager.LightThemeSource };

        static AppearanceSettings()
        {
            // add the default themes
            themes.Add(DarkTheme);
            themes.Add(LightTheme);
        }

        public AppearanceSettings()
        {
            InitializeComponent();

            SelectedFontSize = AppearanceManager.Current.FontSize == FontSizeEnum.Large ? FontLarge : FontSmall;
            SyncThemeAndColor();

            AppearanceManager.Current.PropertyChanged += OnAppearanceManagerPropertyChanged;

            // a simple view model for appearance configuration
            DataContext = this;
        }

        const string FontSmall = "Small",
            FontLarge = "Large";

        static readonly Color[] AccentColorCollection = new Color[]
        {
            Color.FromRgb(0xa4, 0xc4, 0x00),   // lime
            Color.FromRgb(0x8c, 0xbf, 0x26),   // lime
            Color.FromRgb(0x60, 0xa9, 0x17),   // green
            Color.FromRgb(0x33, 0x99, 0x33),   // green  
            Color.FromRgb(0x00, 0x8a, 0x00),   // emerald
            Color.FromRgb(0x00, 0xab, 0xa9),   // teal
            Color.FromRgb(0x1b, 0xa1, 0xe2),   // cyan
            Color.FromRgb(0x33, 0x99, 0xff),   // blue
            Color.FromRgb(0x00, 0x50, 0xef),   // cobalt
            Color.FromRgb(0x6a, 0x00, 0xff),   // indigo
            Color.FromRgb(0xf4, 0x72, 0xd0),   // pink
            Color.FromRgb(0xd8, 0x00, 0x73),   // magenta
            Color.FromRgb(0xff, 0x00, 0x97),   // magenta
            Color.FromRgb(0xe3, 0xc8, 0x00),   // yellow
            Color.FromRgb(0xf0, 0xa3, 0x0a),   // amber
            Color.FromRgb(0xf0, 0x96, 0x09),   // orange
            Color.FromRgb(0xfa, 0x68, 0x00),   // orange
            Color.FromRgb(0xff, 0x45, 0x00),   // orange red
            Color.FromRgb(0xe5, 0x14, 0x00),   // red
            Color.FromRgb(0xa2, 0x00, 0x25),   // crimson
            Color.FromRgb(0x82, 0x5a, 0x2c),   // brown
            Color.FromRgb(0x6d, 0x87, 0x64),   // olive
            Color.FromRgb(0x64, 0x76, 0x87),   // steel
            Color.FromRgb(0x76, 0x60, 0x8a),   // mauve
            Color.FromRgb(0x87, 0x79, 0x4e),   // taupe
            Color.FromRgb(0xa2, 0x00, 0xff),   // purple
        };

        static Color selectedAccentColor;
        static LinkCollection themes = new LinkCollection();
        public static Link selectedTheme { get; private set; }
        static string selectedFontSize;

        void SyncThemeAndColor()
        {
            // synchronizes the selected viewmodel theme with the actual theme used by the appearance manager.
            this.SelectedTheme = themes.FirstOrDefault(l => l.Source.Equals(AppearanceManager.Current.ThemeSource));

            // and make sure accent color is up-to-date
            this.SelectedAccentColor = AppearanceManager.Current.AccentColor;
        }

        void OnAppearanceManagerPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "ThemeSource" || e.PropertyName == "AccentColor")
                SyncThemeAndColor();
        }

        public LinkCollection Themes { get { return themes; } }

        public string[] FontSizes { get { return new string[] { FontSmall, FontLarge }; } }

        public Color[] AccentColors { get { return AccentColorCollection; } }

        public Link SelectedTheme
        {
            get { return selectedTheme; }
            set
            {
                if (selectedTheme != value)
                {
                    selectedTheme = value;
                    OnPropertyChanged("SelectedTheme");

                    // and update the actual theme
                    AppearanceManager.Current.ThemeSource = value.Source;
                }
            }
        }

        public string SelectedFontSize
        {
            get { return selectedFontSize; }
            set
            {
                if (selectedFontSize != value)
                {
                    selectedFontSize = value;
                    OnPropertyChanged("SelectedFontSize");

                    AppearanceManager.Current.FontSize = value == FontLarge ? FontSizeEnum.Large : FontSizeEnum.Small;
                }
            }
        }

        public Color SelectedAccentColor
        {
            get { return selectedAccentColor; }
            set
            {
                if (selectedAccentColor != value)
                {
                    selectedAccentColor = value;
                    OnPropertyChanged("SelectedAccentColor");

                    AppearanceManager.Current.AccentColor = value;
                }
            }
        }

        void OnPropertyChanged(string e)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(e));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
