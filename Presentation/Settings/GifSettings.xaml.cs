using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Captura
{
    /// <summary>
    /// Interaction logic for GifSettings.xaml
    /// </summary>
    public partial class GifSettings : INotifyPropertyChanged
    {
        public GifSettings()
        {
            InitializeComponent();

            DataContext = this;
        }

        public static bool UnconstrainedGif;

        public bool _UnconstrainedGif
        {
            get { return UnconstrainedGif; }
            set
            {
                if (UnconstrainedGif == value)
                    return;

                UnconstrainedGif = value;
                OnPropertyChanged();
            }
        }

        public static bool GifRepeat;

        public bool _GifRepeat
        {
            get { return GifRepeat; }
            set
            {
                if (GifRepeat == value)
                    return;

                GifRepeat = value;
                OnPropertyChanged();
            }
        }

        public static int GifRepeatCount;

        public int _RepeatCount
        {
            get { return GifRepeatCount; }
            set
            {
                if (GifRepeatCount == value)
                    return;

                GifRepeatCount = value;
                OnPropertyChanged();
            }
        }

        #region INotifyPropertyChanged
        void OnPropertyChanged([CallerMemberName] string e = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(e));
        }

        public event PropertyChangedEventHandler PropertyChanged;
        #endregion
    }
}
