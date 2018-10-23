namespace Captura.Models
{
    public interface IVideoSourceProvider
    {
        string Name { get; }

        IVideoItem Source { get; }
    }
}