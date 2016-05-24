using Captura.Properties;

namespace Captura
{
    public class GifViewModel : ViewModelBase
    {
        public bool Unconstrained
        {
            get { return Settings.Default.GifUnconstrained; }
            set
            {
                if (Unconstrained == value)
                    return;

                Settings.Default.GifUnconstrained = value;

                OnPropertyChanged();
            }
        }

        bool _repeat;

        public bool Repeat
        {
            get { return _repeat; }
            set
            {
                if (_repeat == value)
                    return;

                _repeat = value;

                OnPropertyChanged();
            }
        }

        int _repeatCount;

        public int RepeatCount
        {
            get { return _repeatCount; }
            set
            {
                if (_repeatCount == value)
                    return;

                _repeatCount = value;

                OnPropertyChanged();
            }
        }
    }
}