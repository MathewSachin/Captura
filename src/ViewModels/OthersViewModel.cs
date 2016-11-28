using Captura.Properties;

namespace Captura
{
    public class OthersViewModel : ViewModelBase
    {
        readonly RegionSelector _regionSelector = RegionSelector.Instance;

        public OthersViewModel()
        {
            _regionSelector.Closing += (Sender, Args) => Args.Cancel = true;
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
        
        public bool MouseClicks
        {
            get { return Settings.Default.MouseClicks; }
            set
            {
                if (MouseClicks == value)
                    return;

                Settings.Default.MouseClicks = value;

                OnPropertyChanged();
            }
        }
        
        public bool KeyStrokes
        {
            get { return Settings.Default.KeyStrokes; }
            set
            {
                if (KeyStrokes == value)
                    return;

                Settings.Default.KeyStrokes = value;

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
        
        public bool Cursor
        {
            get { return Settings.Default.IncludeCursor; }
            set
            {
                if (Cursor == value)
                    return;

                Settings.Default.IncludeCursor = value;

                OnPropertyChanged();
            }
        }
        
        public bool MinimizeOnStart
        {
            get { return Settings.Default.MinimizeOnStart; }
            set
            {
                if (MinimizeOnStart == value)
                    return;

                Settings.Default.MinimizeOnStart = value;

                OnPropertyChanged();
            }
        }
    }
}