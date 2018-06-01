using System.IO;

namespace Captura.Models
{
    public class LicenseItem
    {
        readonly string _fileName;

        public LicenseItem(string FileName)
        {
            _fileName = FileName;

            Name = Path.GetFileNameWithoutExtension(FileName);
        }

        public string Name { get; }

        public string Content => File.ReadAllText(_fileName);
    }
}