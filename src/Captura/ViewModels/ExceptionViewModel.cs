using System;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Captura
{
    public class ExceptionViewModel : NotifyPropertyChanged
    {
        public ExceptionViewModel()
        {
            CopyToClipboardCommand = new DelegateCommand(() =>
            {
                if (Exceptions.Count > 0)
                {
                    Exceptions[0].ToString().WriteToClipboard();
                }
            });
        }

        public void Init(Exception Exception)
        {
            while (Exception != null)
            {
                Exceptions.Add(Exception);

                Exception = Exception.InnerException;
            }

            SelectedException = Exceptions[0];
        }

        public ObservableCollection<Exception> Exceptions { get; } = new ObservableCollection<Exception>();

        Exception _selectedException;

        public Exception SelectedException
        {
            get => _selectedException;
            set
            {
                _selectedException = value;
                
                OnPropertyChanged();
            }
        }

        public ICommand CopyToClipboardCommand { get; }
    }
}