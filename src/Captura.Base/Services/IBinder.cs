using System;

namespace Captura
{
    public interface IBinder
    {
        void BindSingleton<T>();
        void Bind<TFrom, TTarget>(bool Singleton = true) where TTarget : TFrom;
        void Bind<T>(Func<T> Function, bool Singleton = true);

        T Get<T>();
    }
}