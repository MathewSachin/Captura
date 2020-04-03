namespace Captura.Video
{
    public class WindowItem : NotifyPropertyChanged, IVideoItem
    {
        readonly IPlatformServices _platformServices;

        public IWindow Window { get; }

        public WindowItem(IWindow Window, IPlatformServices PlatformServices)
        {
            this.Window = Window;
            _platformServices = PlatformServices;
            Name = Window.Title;
        }

        public override string ToString() => Name;

        public string Name { get; }

        public IImageProvider GetImageProvider(bool IncludeCursor)
        {
            if (!Window.IsAlive)
            {
                throw new WindowClosedException();
            }

            return _platformServices.GetWindowProvider(Window, IncludeCursor);
        }
    }
}