using System;
using Ninject.Modules;

namespace Captura
{
    class Binder : NinjectModule, IBinder
    {
        static int _currentIndex;
        readonly int _index;
        readonly IModule _module;

        public Binder(IModule Module)
        {
            _index = _currentIndex++;
            _module = Module;
        }

        public void BindSingleton<T>()
        {
            Bind<T>().ToSelf().InSingletonScope();
        }

        public void Bind<TFrom, TTarget>(bool Singleton = true) where TTarget : TFrom
        {
            var binding = Bind<TFrom>().To<TTarget>();

            if (Singleton)
                binding.InSingletonScope();
        }

        public void Bind<T>(Func<T> Function, bool Singleton = true)
        {
            var binding = Bind<T>().ToMethod(M => Function());

            if (Singleton)
                binding.InSingletonScope();
        }

        public override void Load()
        {
            _module.OnLoad(this);
        }

        public T Get<T>() => ServiceProvider.Get<T>();

        public override string Name => $"Module{_index}";
    }
}