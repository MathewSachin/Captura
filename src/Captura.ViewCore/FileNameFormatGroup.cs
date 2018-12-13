namespace Captura.Models
{
    public class FileNameFormatGroup
    {
        public FileNameFormatGroup(string Name, FileNameFormatItem[] Formats)
        {
            this.Name = Name;
            this.Formats = Formats;
        }

        public string Name { get; }

        public FileNameFormatItem[] Formats { get; }
    }
}