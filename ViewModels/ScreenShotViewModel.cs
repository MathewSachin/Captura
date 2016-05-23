using System.Drawing.Imaging;

namespace Captura
{
    public class ScreenShotViewModel : ViewModelBase
    {
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

        ImageFormat _imageFormat = ImageFormat.Png;

        public ImageFormat SelectedImageFormat
        {
            get { return _imageFormat; }
            set
            {
                if (_imageFormat == value)
                    return;

                _imageFormat = value;

                OnPropertyChanged();
            }
        }

        public string[] SaveTo => new[] { "Disk", "Clipboard" };

        string _saveTo = "Disk";

        public string SelectedSaveTo
        {
            get { return _saveTo; }
            set
            {
                if (_saveTo == value)
                    return;

                _saveTo = value;

                OnPropertyChanged();
            }
        }

        bool _resize;

        public bool DoResize
        {
            get { return _resize; }
            set
            {
                if (_resize == value)
                    return;

                _resize = value;

                OnPropertyChanged();
            }
        }

        int _resizeWidth = 640;

        public int ResizeWidth
        {
            get { return _resizeWidth; }
            set
            {
                if (_resizeWidth == value)
                    return;

                _resizeWidth = value;

                OnPropertyChanged();
            }
        }

        int _resizeHeight = 400;

        public int ResizeHeight
        {
            get { return _resizeHeight; }
            set
            {
                if (_resizeHeight == value)
                    return;

                _resizeHeight = value;

                OnPropertyChanged();
            }
        }
    }
}