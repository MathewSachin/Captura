using System;
using System.Threading;

namespace Captura.Models
{
    public class SyncContextManager
    {
        readonly SynchronizationContext _syncContext = SynchronizationContext.Current;

        public void Run(Action Action, bool Async = false)
        {
            if (_syncContext == null)
            {
                Action();
            }
            else if (Async)
            {
                _syncContext.Post(D => Action(), null);
            }
            else _syncContext.Send(D => Action(), null);
        }

        public T Run<T>(Func<T> Action)
        {
            if (_syncContext == null)
            {
                return Action();
            }

            T result = default;

            _syncContext.Send(D => result = Action(), null);

            return result;
        }
    }
}