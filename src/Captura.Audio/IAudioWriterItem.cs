namespace Captura.Audio
{
    public interface IAudioWriterItem
    {
        string Name { get; }

        string Extension { get; }

        IAudioFileWriter GetAudioFileWriter(string FileName, WaveFormat Wf, int AudioQuality);
    }
}