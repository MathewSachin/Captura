namespace Captura.Models
{
    public static class GifskiModule
    {
        public static void Load(IBinder Binder)
        {
            Binder.Bind<IVideoConverter>(() => new GifskiVideoConverter());
        }
    }
}