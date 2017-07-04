using Screna.Audio;

namespace Captura.Models
{
    public class WaveItem : NoVideoItem
    {
        public static WaveItem Instance { get; } = new WaveItem();

        WaveItem() : base("Wave", ".wav") { }

        public override IAudioFileWriter GetAudioFileWriter(string FileName, WaveFormat Wf, int AudioQuality)
        {
            return new AudioFileWriter(FileName, Wf);
        }
    }
}
