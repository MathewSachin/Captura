using Captura.Models;
using Captura.ViewModels;

namespace Captura
{
    public static class FFmpegModule
    {
        public static void Load(IBinder Binder)
        {
            Binder.BindSingleton<FFmpegSettings>();
            Binder.BindSingleton<FFmpegCodecsViewModel>();
            Binder.BindSingleton<FFmpegLog>();
            Binder.BindAsInterfaceAndClass<IVideoWriterProvider, FFmpegWriterProvider>();
            Binder.BindAsInterfaceAndClass<IVideoWriterProvider, StreamingWriterProvider>();

            foreach (var audioItem in FFmpegAudioItem.Items)
            {
                Binder.Bind<IAudioWriterItem>(() => audioItem);
            }
        }
    }
}