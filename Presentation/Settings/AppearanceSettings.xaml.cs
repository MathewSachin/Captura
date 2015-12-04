using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Captura
{
    public partial class AppearanceSettings : UserControl, INotifyPropertyChanged
    {
        const string KeyAccentColor = "AccentColor",
            KeyAccent = "Accent";

        static ResourceDictionary GetThemeDictionary()
        {
            // determine the current theme by looking at the app resources and return the first dictionary having the resource key 'WindowBackground' defined.
            return (from dict in Application.Current.Resources.MergedDictionaries
                    where dict.Contains("WindowBackground")
                    select dict).FirstOrDefault();
        }

        static Uri GetThemeSource()
        {
            var dict = GetThemeDictionary();
            if (dict != null) return dict.Source;

            // could not determine the theme dictionary
            return null;
        }

        static void SetThemeSource(Uri source)
        {
            var oldThemeDict = GetThemeDictionary();
            var dictionaries = Application.Current.Resources.MergedDictionaries;
            var themeDict = new ResourceDictionary { Source = source };

            // if theme defines an accent color, use it
            var accentColor = themeDict[KeyAccentColor] as Color?;
            if (accentColor.HasValue)
                // remove from the theme dictionary and apply globally if useThemeAccentColor is true
                themeDict.Remove(KeyAccentColor);

            // add new before removing old theme to avoid dynamicresource not found warnings
            dictionaries.Add(themeDict);

            // remove old theme
            if (oldThemeDict != null) dictionaries.Remove(oldThemeDict);
        }

        /// <summary>
        /// Gets or sets the accent color.
        /// </summary>
        internal static Color AccentColor
        {
            get
            {
                var accentColor = Application.Current.Resources[KeyAccentColor] as Color?;

                if (accentColor.HasValue)
                    return accentColor.Value;

                // default color: Blue
                return Color.FromRgb(0x33, 0x99, 0xff);
            }
            set
            {
                // set accent color and brush resources
                Application.Current.Resources[KeyAccentColor] = value;
                Application.Current.Resources[KeyAccent] = new SolidColorBrush(value);

                // re-apply theme to ensure brushes referencing AccentColor are updated
                var themeSource = GetThemeSource();
                if (themeSource != null) SetThemeSource(themeSource);
            }
        }

        public AppearanceSettings()
        {
            InitializeComponent();
            
            DataContext = this;
        }

        static readonly Color[] AccentColorCollection = new Color[]
        {
            Color.FromArgb(0xd7, 0x8c, 0xbf, 0x26),   // lime
            Color.FromArgb(0xd7, 0x33, 0x99, 0x33),   // green  
            Color.FromArgb(0xd7, 0x00, 0x8a, 0x00),   // emerald
            Color.FromArgb(0xd7, 0x33, 0x99, 0xff),   // blue
            Color.FromArgb(0xd7, 0x00, 0x50, 0xef),   // cobalt
            Color.FromArgb(0xd7, 0x6a, 0x00, 0xff),   // indigo
            Color.FromArgb(0xd7, 0xf4, 0x72, 0xd0),   // pink
            Color.FromArgb(0xd7, 0xe3, 0xc8, 0x00),   // yellow
            Color.FromArgb(0xd7, 0xf0, 0xa3, 0x0a),   // amber
            Color.FromArgb(0xd7, 0xf0, 0x96, 0x09),   // orange
            Color.FromArgb(0xd7, 0xfa, 0x68, 0x00),   // orange
            Color.FromArgb(0xd7, 0xff, 0x45, 0x00),   // orange red
            Color.FromArgb(0xd7, 0xe5, 0x14, 0x00),   // red
            Color.FromArgb(0xd7, 0xa2, 0x00, 0x25),   // crimson
            Color.FromArgb(0xd7, 0x82, 0x5a, 0x2c),   // brown
        };

        public Color[] AccentColors { get { return AccentColorCollection; } }

        static Color _SelectedAccentColor = AccentColor;

        public Color SelectedAccentColor
        {
            get { return _SelectedAccentColor; }
            set
            {
                if (_SelectedAccentColor != value)
                {
                    _SelectedAccentColor = value;
                    OnPropertyChanged("SelectedAccentColor");

                    AccentColor = value;
                }
            }
        }

        #region INotifyPropertyChanged
        void OnPropertyChanged(string e)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(e));
        }

        public event PropertyChangedEventHandler PropertyChanged;
        #endregion
    }
}
