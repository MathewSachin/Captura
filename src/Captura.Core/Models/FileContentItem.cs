using System.IO;

namespace Captura.Models
{
    public class FileContentItem
    {
        public string FileName { get; }

        public FileContentItem(string FileName)
        {
            this.FileName = FileName;

            Name = Path.GetFileNameWithoutExtension(FileName);
        }

        public string Name { get; }

        public string Content => File.Exists(FileName) ? File.ReadAllText(FileName) : null;
    }
}