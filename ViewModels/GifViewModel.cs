namespace Captura
{
    public class GifViewModel : ViewModelBase
    {
        bool _unconstrained;

        public bool Unconstrained
        {
            get { return _unconstrained; }
            set
            {
                if (_unconstrained == value)
                    return;

                _unconstrained = value;

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