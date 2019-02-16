using Captura.Audio;
using Screna.Audio;

namespace Captura.Models
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class WaveItem : IAudioWriterItem
    {
        WaveItem() { }

        public static IAudioWriterItem Instance { get; } = new WaveItem();

        public string Name { get; } = "Wave";
        public string Extension { get; } = ".wav";

        public IAudioFileWriter GetAudioFileWriter(string FileName, WaveFormat Wf, int AudioQuality)
        {
            return new AudioFileWriter(FileName, Wf);
        }
    }
}
