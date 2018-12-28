using System;

namespace Captura
{
    public interface IModule : IDisposable
    {
        void OnLoad(IBinder Binder);
    }
}