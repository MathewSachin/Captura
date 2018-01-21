using Captura.Models;
using Captura.ViewModels;
using Ninject.Modules;

namespace Captura
{
    public class CoreModule : NinjectModule
    {
        public override void Load()
        {
            // Singleton View Models
            Bind<MainViewModel>().ToSelf().InSingletonScope();
            Bind<VideoViewModel>().ToSelf().InSingletonScope();
            Bind<AudioViewModel>().ToSelf().InSingletonScope();

            // Settings
            Bind<Settings>().ToSelf().InSingletonScope();

            // Localization
            Bind<LanguageManager>().ToMethod(M => LanguageManager.Instance).InSingletonScope();

            // Image Writers
            Bind<IImageWriterItem>().To<DiskWriter>().InSingletonScope();
            Bind<IImageWriterItem>().To<ClipboardWriter>().InSingletonScope();
            Bind<IImageWriterItem>().To<ImgurWriter>().InSingletonScope();
            
            // Audio Source
            if (BassAudioSource.Available)
            {
                Bind<AudioSource>().To<BassAudioSource>().InSingletonScope();
            }
            else Bind<AudioSource>().To<NoAudioSource>().InSingletonScope();
        }
    }
}