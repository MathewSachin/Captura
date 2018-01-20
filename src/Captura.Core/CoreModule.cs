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
            Bind<VideoViewModel>().ToSelf().InSingletonScope();
            Bind<AudioViewModel>().ToSelf().InSingletonScope();
            Bind<DiskWriter>().ToSelf().InSingletonScope();
            
            if (BassAudioSource.Available)
            {
                Bind<AudioSource>().To<BassAudioSource>().InSingletonScope();
            }
            else Bind<AudioSource>().To<NoAudioSource>().InSingletonScope();
        }
    }
}