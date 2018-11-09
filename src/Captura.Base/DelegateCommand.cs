using System;
using System.Threading;
using System.Windows.Input;

namespace Captura
{
    public class DelegateCommand : ICommand
    {
        readonly Action<object> _execute;
        bool _canExecute;
        readonly SynchronizationContext _syncContext = SynchronizationContext.Current;
        
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

            void Do()
            {
                CanExecuteChanged?.Invoke(this, EventArgs.Empty);
            }

            if (_syncContext != null)
            {
                _syncContext.Post(S => Do(), null);
            }
            else Do();
        }

        public event EventHandler CanExecuteChanged;
    }
}