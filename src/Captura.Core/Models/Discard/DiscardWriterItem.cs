using Screna;

namespace Captura.Models
{
    public class DiscardWriterItem : IVideoWriterItem
    {
        public string Extension { get; } = "";

        public IVideoFileWriter GetVideoFileWriter(VideoWriterArgs Args)
        {
            return new DiscardWriter();
        }

        public override string ToString() => "Discard";
    }
}