using Screna;
using Screna.Audio;

namespace Captura
{
    public class GifItem : IVideoWriterItem
    {
        public static GifItem Instance { get; } = new GifItem();

        GifItem() { }

        public string Extension { get; } = ".gif";

        public override string ToString() => "Gif";

        public IVideoFileWriter GetVideoFileWriter(string FileName, int FrameRate, IImageProvider ImageProvider, IAudioProvider AudioProvider)
        {
            var repeat = App.MainViewModel.GifViewModel.Repeat ? App.MainViewModel.GifViewModel.RepeatCount : -1;
            
            return new GifWriter(FileName, 1000 / App.MainViewModel.VideoViewModel.FrameRate, repeat);
        }
    }
}
