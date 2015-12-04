using System.ComponentModel;
using System.Drawing.Imaging;
using System.Windows.Controls;
using System;

namespace Captura
{
    public partial class ScreenShotSettings : UserControl, INotifyPropertyChanged
    {
        static ScreenShotSettings Instance;

        public ScreenShotSettings()
        {
            InitializeComponent();

            DataContext = this;

            Instance = this;
        }

        static string selectedSaveTo = "Disk";

        public static ImageFormat SelectedImageFormat = ImageFormat.Png;
        
        public static bool SaveToClipboard { get { return selectedSaveTo == "Clipboard"; } }

        public string[] SaveTo { get { return new string[] { "Disk", "Clipboard" }; } }

        public ImageFormat[] ImageFormats
        {
            get
            {
                return new ImageFormat[]
                {
                    ImageFormat.Png,
                    ImageFormat.Jpeg,
                    ImageFormat.Bmp,
                    ImageFormat.Tiff,
                    ImageFormat.Wmf,
                    ImageFormat.Exif,
                    ImageFormat.Gif,
                    ImageFormat.Icon,
                    ImageFormat.Emf
                };
            }
        }
        
        public string _SelectedSaveTo
        {
            get { return selectedSaveTo; }
            set
            {
                if (selectedSaveTo != value)
                {
                    selectedSaveTo = value;
                    OnPropertyChanged("_SelectedSaveTo");
                }
            }
        }

        public ImageFormat _SelectedImageFormat
        {
            get { return SelectedImageFormat; }
            set
            {
                if (SelectedImageFormat != value)
                {
                    SelectedImageFormat = value;
                    OnPropertyChanged("_SelectedImageFormat");
                }
            }
        }

        public static bool DoResize;

        #region Resize
        public bool _DoResize
        {
            get { return DoResize; }
            set
            {
                if (DoResize != value)
                {
                    DoResize = value;
                    OnPropertyChanged("_DoResize");
                }
            }
        }

        public static int ResizeWidth
        {
            get
            {
                return Instance == null ? 640
                    : (int)Instance.ResizeWidthBox.Dispatcher.Invoke(
                    new Func<int>(() => Instance.ResizeWidthBox.Value));
            }
        }

        public static int ResizeHeight
        {
            get
            {
                return Instance == null ? 400
                    : (int)Instance.ResizeHeightBox.Dispatcher.Invoke(
                    new Func<int>(() => Instance.ResizeHeightBox.Value));
            }
        }
        #endregion

        void OnPropertyChanged(string e)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(e));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
