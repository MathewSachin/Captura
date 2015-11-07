using System.ComponentModel;
using System.Linq;
using System.Windows.Media;
using FirstFloor.ModernUI.Presentation;

namespace Captura
{
    /// <summary>
    /// A simple view model for configuring theme, font and accent colors.
    /// </summary>
    public class AppearanceSettingsViewModel : NotifyPropertyChanged
    {
        const string FontSmall = "small";
        const string FontLarge = "large";

        const string PaletteMetro = "metro";
        const string PaletteWP = "windows phone";

        // 9 accent colors from metro design principles
        static readonly Color[] metroAccentColors = new Color[]
        {
            Color.FromRgb(0x33, 0x99, 0xff),   // blue
            Color.FromRgb(0x00, 0xab, 0xa9),   // teal
            Color.FromRgb(0x33, 0x99, 0x33),   // green
            Color.FromRgb(0x8c, 0xbf, 0x26),   // lime
            Color.FromRgb(0xf0, 0x96, 0x09),   // orange
            Color.FromRgb(0xff, 0x45, 0x00),   // orange red
            Color.FromRgb(0xe5, 0x14, 0x00),   // red
            Color.FromRgb(0xff, 0x00, 0x97),   // magenta
            Color.FromRgb(0xa2, 0x00, 0xff),   // purple            
        };

        // 20 accent colors from Windows Phone 8
        static readonly Color[] wpAccentColors = new Color[]
        {
            Color.FromRgb(0xa4, 0xc4, 0x00),   // lime
            Color.FromRgb(0x60, 0xa9, 0x17),   // green
            Color.FromRgb(0x00, 0x8a, 0x00),   // emerald
            Color.FromRgb(0x00, 0xab, 0xa9),   // teal
            Color.FromRgb(0x1b, 0xa1, 0xe2),   // cyan
            Color.FromRgb(0x00, 0x50, 0xef),   // cobalt
            Color.FromRgb(0x6a, 0x00, 0xff),   // indigo
            Color.FromRgb(0xaa, 0x00, 0xff),   // violet
            Color.FromRgb(0xf4, 0x72, 0xd0),   // pink
            Color.FromRgb(0xd8, 0x00, 0x73),   // magenta
            Color.FromRgb(0xa2, 0x00, 0x25),   // crimson
            Color.FromRgb(0xe5, 0x14, 0x00),   // red
            Color.FromRgb(0xfa, 0x68, 0x00),   // orange
            Color.FromRgb(0xf0, 0xa3, 0x0a),   // amber
            Color.FromRgb(0xe3, 0xc8, 0x00),   // yellow
            Color.FromRgb(0x82, 0x5a, 0x2c),   // brown
            Color.FromRgb(0x6d, 0x87, 0x64),   // olive
            Color.FromRgb(0x64, 0x76, 0x87),   // steel
            Color.FromRgb(0x76, 0x60, 0x8a),   // mauve
            Color.FromRgb(0x87, 0x79, 0x4e),   // taupe
        };

        static string selectedPalette = PaletteWP;

        static Color selectedAccentColor;
        static LinkCollection themes = new LinkCollection();
        static Link selectedTheme;
        static string selectedFontSize;

        static AppearanceSettingsViewModel()
        {
            // add the default themes
            themes.Add(new Link { DisplayName = "dark", Source = AppearanceManager.DarkThemeSource });
            themes.Add(new Link { DisplayName = "light", Source = AppearanceManager.LightThemeSource });
        }

        public AppearanceSettingsViewModel()
        {
            this.SelectedFontSize = AppearanceManager.Current.FontSize == FontSize.Large ? FontLarge : FontSmall;
            SyncThemeAndColor();

            AppearanceManager.Current.PropertyChanged += OnAppearanceManagerPropertyChanged;
        }

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

        public string[] Palettes { get { return new string[] { PaletteMetro, PaletteWP }; } }

        public Color[] AccentColors { get { return selectedPalette == PaletteMetro ? metroAccentColors : wpAccentColors; } }

        public string SelectedPalette
        {
            get { return selectedPalette; }
            set
            {
                if (selectedPalette != value)
                {
                    selectedPalette = value;
                    OnPropertyChanged("AccentColors");

                    this.SelectedAccentColor = this.AccentColors.FirstOrDefault();
                }
            }
        }

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

                    AppearanceManager.Current.FontSize = value == FontLarge ? FontSize.Large : FontSize.Small;
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
    }
}
