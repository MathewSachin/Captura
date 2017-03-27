using Screna.Audio;

namespace Captura
{
    public class FFMpegAudioWriterItem : IAudioWriterItem
    {
        readonly string _name;
        FFMpegAudioArgsProvider _audioArgsProvider;

        public string Extension { get; }

        FFMpegAudioWriterItem(string Name, string Extension, FFMpegAudioArgsProvider AudioArgsProvider)
        {
            _name = Name;
            _audioArgsProvider = AudioArgsProvider;

            this.Extension = Extension;            
        }

        public override string ToString() => _name;
        
        public IAudioFileWriter GetAudioFileWriter(string FileName, WaveFormat Wf, int AudioQuality)
        {
            return new FFMpegAudioWriter(FileName, AudioQuality, _audioArgsProvider, Wf.SampleRate, Wf.Channels);
        }

        public static FFMpegAudioArgsProvider Aac { get; } = q =>
        {
            // bitrate: 32k to 512k (steps of 32k)
            var b = 32 * (1 + (15 * (q - 1)) / 99);

            return $"-acodec aac -strict -2 -ac 2 -b:a {b}k";
        };

        public static FFMpegAudioArgsProvider Mp3 { get; } = q =>
        {
            // quality: 9 (lowest) to 0 (highest)
            var qscale = (100 - q) / 11;

            return $"-c:a libmp3lame -qscale:a {qscale}";
        };

        public static FFMpegAudioArgsProvider Vorbis { get; } = q =>
        {
            // quality: 0 (lowest) to 10 (highest)
            var qscale = (10 * (q - 1)) / 99;

            return $"-c:a libvorbis -qscale:a {qscale}";
        };

        public static FFMpegAudioWriterItem[] Items { get; } = new[]
        {
            new FFMpegAudioWriterItem("AAC", ".aac", Aac),
            new FFMpegAudioWriterItem("Mp3", ".mp3", Mp3),
            new FFMpegAudioWriterItem("Vorbis", ".ogg", Vorbis)
        };
    }
}