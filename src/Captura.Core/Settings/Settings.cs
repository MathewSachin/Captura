using Captura.Models;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;

namespace Captura
{
    public partial class Settings : ApplicationSettingsBase
    {
        [UserScopedSetting]
        [DefaultSettingValue(null)]
        public string AccentColor
        {
            get => Get<string>();
            set => Set(value);
        }

        [UserScopedSetting]
        [DefaultSettingValue("200")]
        public int MainWindowLeft
        {
            get => Get<int>();
            set => Set(value);
        }

        [UserScopedSetting]
        [DefaultSettingValue("200")]
        public int MainWindowTop
        {
            get => Get<int>();
            set => Set(value);
        }

        [UserScopedSetting]
        [DefaultSettingValue("True")]
        public bool Expanded
        {
            get => Get<bool>();
            set => Set(value);
        }

        [UserScopedSetting]
        public List<RecentItemModel> RecentItems
        {
            get => Get<List<RecentItemModel>>();
            set => Set(value);
        }
        
        [UserScopedSetting]
        [DefaultSettingValue("30")]
        public int RecentMax
        {
            get => Get<int>();
            set => Set(value);
        }
        
        [UserScopedSetting]
        public string OutPath
        {
            get => Get<string>();
            set => Set(value);
        }
        
        [UserScopedSetting]
        public string FFMpegFolder
        {
            get => Get<string>();
            set
            {
                Set(value);

                ServiceProvider.RaiseFFMpegPathChanged();
            }
        }

        [UserScopedSetting]
        [DefaultSettingValue("True")]
        public bool IncludeCursor
        {
            get => Get<bool>();
            set => Set(value);
        }

        [UserScopedSetting]
        [DefaultSettingValue("False")]
        public bool MinimizeOnStart
        {
            get => Get<bool>();
            set => Set(value);
        }
        
        [UserScopedSetting]
        [DefaultSettingValue("70")]
        public int VideoQuality
        {
            get => Get<int>();
            set => Set(value);
        }

        [UserScopedSetting]
        [DefaultSettingValue("10")]
        public int FrameRate
        {
            get => Get<int>();
            set => Set(value);
        }

        [UserScopedSetting]
        [DefaultSettingValue("False")]
        public bool MouseClicks
        {
            get => Get<bool>();
            set => Set(value);
        }

        [UserScopedSetting]
        [DefaultSettingValue("False")]
        public bool KeyStrokes
        {
            get => Get<bool>();
            set => Set(value);
        }
        
        [UserScopedSetting]
        [DefaultSettingValue("en-US")]
        public string Language
        {
            get => Get<string>();
            set => Set(value);
        }

        [UserScopedSetting]
        public List<HotkeyModel> Hotkeys
        {
            get => Get<List<HotkeyModel>>();
            set => Set(value);
        }

        [UserScopedSetting]
        [DefaultSettingValue("False")]
        public bool MinimizeToTray
        {
            get => Get<bool>();
            set => Set(value);
        }

        [UserScopedSetting]
        [DefaultSettingValue("50")]
        public int AudioQuality
        {
            get => Get<int>();
            set => Set(value);
        }

        [UserScopedSetting]
        [DefaultSettingValue("True")]
        public bool TrayNotify
        {
            get => Get<bool>();
            set => Set(value);
        }

        #region Transforms
        [UserScopedSetting]
        [DefaultSettingValue("False")]
        public bool DoResize
        {
            get => Get<bool>();
            set => Set(value);
        }
        
        [UserScopedSetting]
        [DefaultSettingValue("640")]
        public int ResizeWidth
        {
            get => Get<int>();
            set => Set(value);
        }
        
        [UserScopedSetting]
        [DefaultSettingValue("400")]
        public int ResizeHeight
        {
            get => Get<int>();
            set => Set(value);
        }

        [UserScopedSetting]
        [DefaultSettingValue("False")]
        public bool FlipHorizontal
        {
            get => Get<bool>();
            set => Set(value);
        }

        [UserScopedSetting]
        [DefaultSettingValue("False")]
        public bool FlipVertical
        {
            get => Get<bool>();
            set => Set(value);
        }

        [UserScopedSetting]
        [DefaultSettingValue("RotateNone")]
        public RotateBy RotateBy
        {
            get => Get<RotateBy>();
            set => Set(value);
        }
        #endregion

        #region Gif
        [UserScopedSetting]
        [DefaultSettingValue("False")]
        public bool GifRepeat
        {
            get => Get<bool>();
            set => Set(value);
        }
        
        [UserScopedSetting]
        [DefaultSettingValue("0")]
        public int GifRepeatCount
        {
            get => Get<int>();
            set => Set(value);
        }

        [UserScopedSetting]
        [DefaultSettingValue("False")]
        public bool GifVariable
        {
            get => Get<bool>();
            set => Set(value);
        }
        #endregion
        
        [UserScopedSetting]
        [DefaultSettingValue("DarkGray")]
        public Color MouseClick_Color
        {
            get => Get<Color>();
            set => Set(value);
        }
        
        [UserScopedSetting]
        [DefaultSettingValue("25")]
        public float MouseClick_Radius
        {
            get => Get<float>();
            set => Set(value);
        }

        [UserScopedSetting]
        [DefaultSettingValue("0")]
        public float MouseClick_Border
        {
            get => Get<float>();
            set => Set(value);
        }

        [UserScopedSetting]
        [DefaultSettingValue("Black")]
        public Color MouseClick_BorderColor
        {
            get => Get<Color>();
            set => Set(value);
        }

        [UserScopedSetting]
        [DefaultSettingValue("0")]
        public float Keystrokes_Border
        {
            get => Get<float>();
            set => Set(value);
        }

        [UserScopedSetting]
        [DefaultSettingValue("Black")]
        public Color Keystrokes_BorderColor
        {
            get => Get<Color>();
            set => Set(value);
        }

        [UserScopedSetting]
        [DefaultSettingValue("Black")]
        public Color Keystrokes_Color
        {
            get => Get<Color>();
            set => Set(value);
        }

        [UserScopedSetting]
        [DefaultSettingValue("DarkGray")]
        public Color KeystrokesRect_Color
        {
            get => Get<Color>();
            set => Set(value);
        }
        
        [UserScopedSetting]
        [DefaultSettingValue("20")]
        public int Keystrokes_FontSize
        {
            get => Get<int>();
            set => Set(value);
        }

        [UserScopedSetting]
        [DefaultSettingValue("15")]
        public int Keystrokes_MaxLength
        {
            get => Get<int>();
            set => Set(value);
        }

        [UserScopedSetting]
        [DefaultSettingValue("15")]
        public int Keystrokes_PaddingX
        {
            get => Get<int>();
            set => Set(value);
        }

        [UserScopedSetting]
        [DefaultSettingValue("15")]
        public int Keystrokes_PaddingY
        {
            get => Get<int>();
            set => Set(value);
        }

        [UserScopedSetting]
        [DefaultSettingValue("10")]
        public int Keystrokes_CornerRadius
        {
            get => Get<int>();
            set => Set(value);
        }

        // Not a Setting
        public KeyValuePair<Alignment, string>[] XAlignments { get; } = new[]
        {
            new KeyValuePair<Alignment, string>(Alignment.Start, "Left"),
            new KeyValuePair<Alignment, string>(Alignment.Center, "Center"),
            new KeyValuePair<Alignment, string>(Alignment.End, "Right")
        };

        // Not a Setting
        public KeyValuePair<Alignment, string>[] YAlignments { get; } = new[]
        {
            new KeyValuePair<Alignment, string>(Alignment.Start, "Top"),
            new KeyValuePair<Alignment, string>(Alignment.Center, "Center"),
            new KeyValuePair<Alignment, string>(Alignment.End, "Bottom")
        };

        [UserScopedSetting]
        [DefaultSettingValue("Start")]
        public Alignment Keystrokes_XAlign
        {
            get => Get<Alignment>();
            set => Set(value);
        }
        
        [UserScopedSetting]
        [DefaultSettingValue("End")]
        public Alignment Keystrokes_YAlign
        {
            get => Get<Alignment>();
            set => Set(value);
        }

        [UserScopedSetting]
        [DefaultSettingValue("80")]
        public int Keystrokes_X
        {
            get => Get<int>();
            set => Set(value);
        }

        [UserScopedSetting]
        [DefaultSettingValue("200")]
        public int Keystrokes_Y
        {
            get => Get<int>();
            set => Set(value);
        }

        [UserScopedSetting]
        [DefaultSettingValue("2")]
        public int Keystrokes_MaxSeconds
        {
            get => Get<int>();
            set => Set(value);
        }

        [UserScopedSetting]
        [DefaultSettingValue("#BDBDBD")]
        public string TopBarColor
        {
            get => Get<string>();
            set => Set(value);
        }

        [UserScopedSetting]
        [DefaultSettingValue("3")]
        public int RegionBorderThickness
        {
            get => Get<int>();
            set => Set(value);
        }

        [UserScopedSetting]
        [DefaultSettingValue("5000")]
        public int ScreenShotNotifyTimeout
        {
            get => Get<int>();
            set => Set(value);
        }
    }
}
