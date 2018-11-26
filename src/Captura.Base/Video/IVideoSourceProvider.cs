namespace Captura.Models
{
    public interface IVideoSourceProvider
    {
        string Name { get; }

        string Description { get; }

        string Icon { get; }

        IVideoItem Source { get; }
    }
}