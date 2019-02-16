using Captura.Audio;

namespace Captura.Models
{
    public interface IAudioWriterItem
    {
        string Name { get; }

        string Extension { get; }

        IAudioFileWriter GetAudioFileWriter(string FileName, WaveFormat Wf, int AudioQuality);
    }
}