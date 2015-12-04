using System.ComponentModel;
using System.Drawing.Imaging;
using System.Windows.Controls;

namespace Captura
{
    public partial class ScreenShotSettings : UserControl, INotifyPropertyChanged
    {
        public ScreenShotSettings()
        {
            InitializeComponent();

            DataContext = this;
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

        #region Resize
        public static bool DoResize;
        
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

        public static int ResizeWidth = 640, ResizeHeight = 400;

        public int _ResizeWidth
        {
            get { return ResizeWidth; }
            set
            {
                if (ResizeWidth != value)
                {
                    ResizeWidth = value;
                    OnPropertyChanged("_ResizeWidth");
                }
            }
        }

        public int _ResizeHeight
        {
            get { return ResizeHeight; }
            set
            {
                if (ResizeHeight != value)
                {
                    ResizeHeight = value;
                    OnPropertyChanged("_ResizeHeight");
                }
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
