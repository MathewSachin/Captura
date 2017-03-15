using Screna.Audio;

namespace Captura
{
    public class WaveWriterItem : IAudioWriterItem
    {
        public static WaveWriterItem Instance { get; } = new WaveWriterItem();

        WaveWriterItem() { }

        public string Extension => ".wav";

        public override string ToString() => "Wave";

        public IAudioFileWriter GetAudioFileWriter(string FileName, WaveFormat Wf, int AudioQuality)
        {
            return new AudioFileWriter(FileName, Wf);
        }
    }
}
