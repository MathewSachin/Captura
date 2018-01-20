using Captura.Models;
using Captura.ViewModels;
using Ninject.Modules;

namespace Captura
{
    public class CoreModule : NinjectModule
    {
        public override void Load()
        {
            // Singleton
            Bind<MainViewModel>().ToSelf().InSingletonScope();
            Bind<DiskWriter>().ToSelf().InSingletonScope();
            
            if (BassAudioSource.Available)
            {
                Bind<AudioSource>().To<BassAudioSource>();
            }
            else Bind<AudioSource>().To<NoAudioSource>();
        }
    }
}