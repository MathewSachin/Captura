using Screna.Audio;
using System;

namespace Captura
{
    public class FFMpegAudioWriterItem : IAudioWriterItem
    {
        string _name;

        public string Extension { get; }

        FFMpegAudioWriterItem(string Name, string Extension, Func<string> AudioArgsProvider)
        {
            _name = Name;

            this.Extension = Extension;
            this.AudioArgsProvider = AudioArgsProvider;
        }

        public override string ToString() => _name;

        public Func<string> AudioArgsProvider { get; }

        public IAudioFileWriter GetAudioFileWriter(string FileName, WaveFormat Wf)
        {
            return new FFMpegAudioWriter(FileName, this, Wf.SampleRate, Wf.Channels);
        }

        public static FFMpegAudioWriterItem[] Items { get; } = new[]
        {
            new FFMpegAudioWriterItem("AAC", ".aac", () => "-acodec aac -strict -2 -ac 2 -b:a 192k"),
            new FFMpegAudioWriterItem("Mp3", ".mp3", () => "-c:a libmp3lame -qscale:a 4"),
            new FFMpegAudioWriterItem("Vorbis", ".ogg", () => "-c:a libvorbis -qscale:a 3")
        };
    }
}