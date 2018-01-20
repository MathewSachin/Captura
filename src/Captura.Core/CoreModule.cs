using Captura.Models;
using Ninject.Modules;

namespace Captura
{
    public class CoreModule : NinjectModule
    {
        public override void Load()
        {
            if (BassAudioSource.Available)
            {
                Bind<AudioSource>().To<BassAudioSource>();
            }
            else Bind<AudioSource>().To<NoAudioSource>();
        }
    }
}