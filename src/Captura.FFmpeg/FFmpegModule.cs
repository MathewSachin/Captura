using Captura.Audio;
using Captura.Video;

namespace Captura.FFmpeg
{
    public static class FFmpegModule
    {
        public static void Load(IBinder Binder)
        {
            Binder.BindSingleton<FFmpegSettings>();
            Binder.BindAsInterfaceAndClass<IVideoWriterProvider, FFmpegWriterProvider>();
            Binder.BindAsInterfaceAndClass<IVideoWriterProvider, StreamingWriterProvider>();

            foreach (var audioItem in FFmpegAudioItem.Items)
            {
                Binder.Bind<IAudioWriterItem>(() => audioItem);
            }

            Binder.Bind<IVideoConverter>(() => new FFmpegGifConverter());
            Binder.Bind<IVideoConverter>(() => new FFmpegVideoConverter(new Vp8VideoCodec()));
            Binder.Bind<IVideoConverter>(() => new FFmpegVideoConverter(new Vp9VideoCodec()));
        }
    }
}