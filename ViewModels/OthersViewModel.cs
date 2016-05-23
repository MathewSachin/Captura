namespace Captura
{
    public class OthersViewModel : ViewModelBase
    {
        readonly RegionSelector _regionSelector = RegionSelector.Instance;

        public OthersViewModel()
        {
            _regionSelector.Closing += (Sender, Args) =>
            {
                OnPropertyChanged(nameof(RegionSelectorVisible));

                Args.Cancel = true;
            };
        }

        public bool RegionSelectorVisible
        {
            get { return _regionSelector.IsVisible; }
            set
            {
                if (RegionSelectorVisible == value)
                    return;
                
                if (value)
                    _regionSelector.Show();
                else _regionSelector.Hide();

                OnPropertyChanged();
            }
        }

        int _duration;

        public int Duration
        {
            get { return _duration; }
            set
            {
                if (_duration == value)
                    return;

                _duration = value;

                OnPropertyChanged();
            }
        }

        int _startDelay;

        public int StartDelay
        {
            get { return _startDelay; }
            set
            {
                if (_startDelay == value)
                    return;

                _startDelay = value;

                OnPropertyChanged();
            }
        }

        bool _mouseClicks;

        public bool MouseClicks
        {
            get { return _mouseClicks; }
            set
            {
                if (_mouseClicks == value)
                    return;

                _mouseClicks = value;

                OnPropertyChanged();
            }
        }

        bool _keyStrokes;

        public bool KeyStrokes
        {
            get { return _keyStrokes; }
            set
            {
                if (_keyStrokes == value)
                    return;

                _keyStrokes = value;

                OnPropertyChanged();
            }
        }

        bool _staticRegion;

        public bool StaticRegion
        {
            get { return _staticRegion; }
            set
            {
                if (_staticRegion == value)
                    return;

                _staticRegion = value;

                OnPropertyChanged();
            }
        }

        bool _cursor;

        public bool Cursor
        {
            get { return _cursor; }
            set
            {
                if (_cursor == value)
                    return;

                _cursor = value;

                OnPropertyChanged();
            }
        }
        
        bool _minOnStart;

        public bool MinimizeOnStart
        {
            get { return _minOnStart; }
            set
            {
                if (_minOnStart == value)
                    return;

                _minOnStart = value;

                OnPropertyChanged();
            }
        }
    }
}