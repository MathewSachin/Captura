namespace Captura.Models
{
    public class FileNameFormatItem
    {
        public FileNameFormatItem(string Format, string Description)
        {
            this.Format = Format;
            this.Description = Description;
        }

        public string Format { get; }

        public string Description { get; }
    }
}