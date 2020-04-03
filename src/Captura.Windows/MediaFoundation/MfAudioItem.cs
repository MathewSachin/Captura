using Captura.Audio;
using SharpDX.MediaFoundation;
using System;
using System.Collections.Generic;

namespace Captura.Windows.MediaFoundation
{
    class MfAudioItem : IAudioWriterItem
    {
        Guid _mediaSubtype;

        MfAudioItem(string Name, string Extension, Guid MediaSubtype)
        {
            this.Name = Name + " (Media Foundation)";
            this.Extension = Extension;

            _mediaSubtype = MediaSubtype;
        }

        public static IEnumerable<MfAudioItem> Items { get; } = new[]
        {
            new MfAudioItem("AAC", ".aac", AudioFormatGuids.Aac)
        };

        public string Name { get; }

        public string Extension { get; }

        public override string ToString() => Name;

        public IAudioFileWriter GetAudioFileWriter(string FileName, WaveFormat Wf, int AudioQuality)
        {
            return new MfAudioWriter(FileName, _mediaSubtype, Wf, AudioQuality);
        }
    }
}
