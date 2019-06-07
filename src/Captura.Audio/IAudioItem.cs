namespace Captura.Audio
{
    public interface IAudioItem
    {
        string Name { get; }

        bool IsLoopback { get; }
    }
}
