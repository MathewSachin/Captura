using Screna;
using Screna.Audio;

namespace Captura
{
    public class GifItem : IVideoWriterItem
    {
        // Singleton
        public static GifItem Instance { get; } = new GifItem();

        GifItem() { }

        public string Extension { get; } = ".gif";

        public override string ToString() => "Gif";

        public IVideoFileWriter GetVideoFileWriter(string FileName, int FrameRate, IImageProvider ImageProvider, IAudioProvider AudioProvider)
        {
            var repeat = MainViewModel.Instance.GifViewModel.Repeat ? MainViewModel.Instance.GifViewModel.RepeatCount : -1;
            
            return new GifWriter(FileName, 1000 / MainViewModel.Instance.VideoViewModel.FrameRate, repeat);
        }
    }
}
