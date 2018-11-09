namespace Captura.Models
{
    public class VideoSourceModel
    {
        public VideoSourceModel(IVideoSourceProvider Provider, string Name, string Description, string IconResource)
        {
            this.Provider = Provider;
            this.Name = new TextLocalizer(Name);
            this.Description = Description;
            this.IconResource = IconResource;
        }

        public IVideoSourceProvider Provider { get; }

        public TextLocalizer Name { get; }

        public string Description { get; }

        public string IconResource { get; }
    }
}