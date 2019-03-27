using Captura.Models;

namespace Captura.ViewModels
{
    public class RecordingModelParams
    {
        public IVideoSourceProvider VideoSourceKind { get; set; }

        public IVideoWriterProvider VideoWriterKind { get; set; }

        public IVideoWriterItem VideoWriter { get; set; }
    }
}