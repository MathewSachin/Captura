using System;

namespace Captura
{
    public class TrayProgressViewModel : NotifyPropertyChanged, ITrayProgress
    {
        public Action OnClick { get; private set; }

        public void RegisterClick(Action Handler)
        {
            OnClick = Handler;
        }

        int _progress;

        public int Progress
        {
            get => _progress;
            set
            {
                _progress = value;
                
                OnPropertyChanged();
            }
        }

        string _primaryText, _secondaryText;

        public string PrimaryText
        {
            get => _primaryText;
            set
            {
                _primaryText = value;
                
                OnPropertyChanged();
            }
        }

        public string SecondaryText
        {
            get => _secondaryText;
            set
            {
                _secondaryText = value;
                
                OnPropertyChanged();
            }
        }

        bool _finished, _success;

        public bool Finished
        {
            get => _finished;
            set
            {
                _finished = value;
                
                OnPropertyChanged();
            }
        }

        public bool Success
        {
            get => _success;
            set
            {
                _success = value;
                
                OnPropertyChanged();
            }
        }
    }
}