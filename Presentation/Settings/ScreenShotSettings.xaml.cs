using System.ComponentModel;
using System.Drawing.Imaging;
using System.Runtime.CompilerServices;

namespace Captura
{
    public partial class ScreenShotSettings : INotifyPropertyChanged
    {
        public ScreenShotSettings()
        {
            InitializeComponent();

            DataContext = this;
        }

        static string _selectedSaveTo = "Disk";

        public static ImageFormat SelectedImageFormat = ImageFormat.Png;
        
        public static bool SaveToClipboard => _selectedSaveTo == "Clipboard";

        public string[] SaveTo => new[] { "Disk", "Clipboard" };

        public ImageFormat[] ImageFormats => new[]
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

        public string _SelectedSaveTo
        {
            get { return _selectedSaveTo; }
            set
            {
                if (_selectedSaveTo == value)
                    return;

                _selectedSaveTo = value;
                OnPropertyChanged();
            }
        }

        public ImageFormat _SelectedImageFormat
        {
            get { return SelectedImageFormat; }
            set
            {
                if (SelectedImageFormat == value)
                    return;

                SelectedImageFormat = value;
                OnPropertyChanged();
            }
        }

        #region Resize
        public static bool DoResize;
        
        public bool _DoResize
        {
            get { return DoResize; }
            set
            {
                if (DoResize == value)
                    return;

                DoResize = value;
                OnPropertyChanged();
            }
        }

        public static int ResizeWidth = 640, ResizeHeight = 400;

        public int _ResizeWidth
        {
            get { return ResizeWidth; }
            set
            {
                if (ResizeWidth == value)
                    return;

                ResizeWidth = value;
                OnPropertyChanged();
            }
        }

        public int _ResizeHeight
        {
            get { return ResizeHeight; }
            set
            {
                if (ResizeHeight == value)
                    return;

                ResizeHeight = value;
                OnPropertyChanged();
            }
        }
        #endregion

        void OnPropertyChanged([CallerMemberName] string e = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(e));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
