using Screna.Audio;

namespace Captura
{
    public class FFMpegAudioWriterItem : IAudioWriterItem
    {
        string _name;

        public string Extension { get; }

        FFMpegAudioWriterItem(string Name, string Extension, FFMpegAudioArgsProvider AudioArgsProvider)
        {
            _name = Name;

            this.Extension = Extension;
            this.AudioArgsProvider = AudioArgsProvider;
        }

        public override string ToString() => _name;

        public FFMpegAudioArgsProvider AudioArgsProvider { get; }

        public IAudioFileWriter GetAudioFileWriter(string FileName, WaveFormat Wf, int AudioQuality)
        {
            return new FFMpegAudioWriter(FileName, AudioQuality, this, Wf.SampleRate, Wf.Channels);
        }

        public static FFMpegAudioWriterItem Aac { get; } = new FFMpegAudioWriterItem("AAC", ".aac", q =>
        {
            // bitrate: 32k to 512k (steps of 32k)
            var b = 32 * (1 + (15 * (q - 1)) / 99);

            return $"-acodec aac -strict -2 -ac 2 -b:a {b}k";
        });

        public static FFMpegAudioWriterItem Mp3 { get; } = new FFMpegAudioWriterItem("Mp3", ".mp3", q =>
        {
            // quality: 9 (lowest) to 0 (highest)
            var qscale = (100 - q) / 11;

            return $"-c:a libmp3lame -qscale:a {qscale}";
        });

        public static FFMpegAudioWriterItem Vorbis { get; } = new FFMpegAudioWriterItem("Vorbis", ".ogg", q =>
        {
            // quality: 0 (lowest) to 10 (highest)
            var qscale = (10 * (q - 1)) / 99;

            return $"-c:a libvorbis -qscale:a {qscale}";
        });

        public static FFMpegAudioWriterItem[] Items { get; } = new[]
        {
            Aac,
            Mp3,
            Vorbis            
        };
    }
}