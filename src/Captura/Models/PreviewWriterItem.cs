using Screna;

namespace Captura.Models
{
    public class PreviewWriterItem : IVideoWriterItem
    {
        public string Extension { get; } = "";

        public IVideoFileWriter GetVideoFileWriter(VideoWriterArgs Args)
        {
            return new PreviewWriter();
        }

        public override string ToString() => "Preview";
    }
}