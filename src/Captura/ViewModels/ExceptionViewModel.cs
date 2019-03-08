using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Screna;

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

        string _message = "An unhandled exception occurred. Here are the details.";

        public string Message
        {
            get => _message;
            set => Set(ref _message, value);
        }

        public void Init(Exception Exception, string Msg)
        {
            if (Msg != null)
                Message = Msg;

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
            set => Set(ref _selectedException, value);
        }

        public ICommand CopyToClipboardCommand { get; }
    }
}