using Captura.Audio;
using Captura.Models;

namespace Captura.ViewModels
{
    public class RecordingModelParams
    {
        public IVideoSourceProvider VideoSourceKind { get; set; }

        public IVideoWriterItem VideoWriter { get; set; }

        public IAudioItem Speaker { get; set; }

        public IAudioItem Microphone { get; set; }
    }
}