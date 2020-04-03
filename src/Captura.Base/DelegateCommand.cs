using System;
using System.Windows.Input;
using Captura.Models;

namespace Captura
{
    public class DelegateCommand : ICommand
    {
        readonly Action<object> _execute;
        bool _canExecute;
        readonly SyncContextManager _syncContext = new SyncContextManager();
        
        public DelegateCommand(Action<object> OnExecute, bool CanExecute = true)
        {
            _execute = OnExecute;
            _canExecute = CanExecute;
        }

        public DelegateCommand(Action OnExecute, bool CanExecute = true)
        {
            _execute = O => OnExecute?.Invoke();
            _canExecute = CanExecute;
        }

        public bool CanExecute(object Parameter) => _canExecute;

        public void Execute(object Parameter) => _execute?.Invoke(Parameter);

        public void RaiseCanExecuteChanged(bool CanExecute)
        {
            _canExecute = CanExecute;

            _syncContext.Run(() => CanExecuteChanged?.Invoke(this, EventArgs.Empty));
        }

        public event EventHandler CanExecuteChanged;
    }
}