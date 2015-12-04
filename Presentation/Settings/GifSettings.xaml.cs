using System.ComponentModel;
using System.Windows.Controls;

namespace Captura
{
    /// <summary>
    /// Interaction logic for GifSettings.xaml
    /// </summary>
    public partial class GifSettings : UserControl, INotifyPropertyChanged
    {
        public GifSettings()
        {
            InitializeComponent();

            DataContext = this;
        }

        public static bool UnconstrainedGif = false;

        public bool _UnconstrainedGif
        {
            get { return UnconstrainedGif; }
            set
            {
                if (UnconstrainedGif != value)
                {
                    UnconstrainedGif = value;
                    OnPropertyChanged("_UnconstrainedGif");
                }
            }
        }

        public static bool GifRepeat = false;

        public bool _GifRepeat
        {
            get { return GifRepeat; }
            set
            {
                if (GifRepeat != value)
                {
                    GifRepeat = value;
                    OnPropertyChanged("_GifRepeat");
                }
            }
        }

        public static int GifRepeatCount = 0;

        public int _RepeatCount
        {
            get { return GifRepeatCount; }
            set
            {
                if (GifRepeatCount != value)
                {
                    GifRepeatCount = value;
                    OnPropertyChanged("_RepeatCount");
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
