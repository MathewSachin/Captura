using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Captura
{
    public abstract class ViewModelBase : INotifyPropertyChanged
    {
        static bool _readyToRecord = true;

        public bool ReadyToRecord
        {
            get { return _readyToRecord; }
            set
            {
                if (_readyToRecord == value)
                    return;

                _readyToRecord = value;

                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string PropertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(PropertyName));
        }
    }
}