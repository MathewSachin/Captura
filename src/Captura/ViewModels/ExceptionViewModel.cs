using System;
using System.Collections.ObjectModel;
using System.Reactive.Linq;
using System.Text;
using System.Windows.Input;
using Captura.Models;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;

namespace Captura
{
    public class ExceptionViewModel : NotifyPropertyChanged
    {
        public ExceptionViewModel()
        {
            CopyToClipboardCommand = Exceptions
                .ObserveProperty(M => M.Count)
                .Select(M => M > 0)
                .ToReactiveCommand()
                .WithSubscribe(OnCopyToClipboard);
        }

        void OnCopyToClipboard()
        {
            var sb = new StringBuilder();

            sb.Append(SystemInfo.GetInfo());

            sb.AppendLine();
            sb.Append(Exceptions[0]);

            sb.ToString().WriteToClipboard();
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