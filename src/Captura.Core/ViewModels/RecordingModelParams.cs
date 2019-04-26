using Captura.Models;
using System.Collections.Generic;

namespace Captura.ViewModels
{
    public class RecordingModelParams
    {
        public IVideoSourceProvider VideoSourceKind { get; set; }

        public IVideoWriterProvider VideoWriterKind { get; set; }

        public IVideoWriterItem VideoWriter { get; set; }

        public IEnumerable<IIsActive<IAudioItem>> AudioItems { get; set; }
    }
}