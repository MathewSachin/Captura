namespace Captura
{
    class VerbsModule : IModule
    {
        public void Dispose()
        {
        }

        public void OnLoad(IBinder Binder)
        {
            Binder.Bind<ICmdlineVerb, StartCmdOptions>();
            Binder.Bind<ICmdlineVerb, ShotCmdOptions>();
            Binder.Bind<ICmdlineVerb, FFmpegCmdOptions>();
            Binder.Bind<ICmdlineVerb, ListCmdOptions>();
            Binder.Bind<ICmdlineVerb, UploadCmdOptions>();
        }
    }
}
