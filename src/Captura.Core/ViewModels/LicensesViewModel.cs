using System.IO;
using System.Linq;
using System.Reflection;
using Captura.Models;

namespace Captura.ViewModels
{
    public class LicensesViewModel : NotifyPropertyChanged
    {
        public LicensesViewModel()
        {
            var selfPath = Assembly.GetExecutingAssembly().Location;

            var folder = Path.Combine(Path.GetDirectoryName(selfPath), "licenses");

            if (Directory.Exists(folder))
            {
                Licenses = Directory.EnumerateFiles(folder).Select(FileName => new FileContentItem(FileName)).ToArray();

                if (Licenses.Length > 0)
                {
                    SelectedLicense = Licenses[0];
                }
            }
        }

        public FileContentItem[] Licenses { get; }

        FileContentItem _selectedLicense;

        public FileContentItem SelectedLicense
        {
            get => _selectedLicense;
            set
            {
                _selectedLicense = value;
                
                OnPropertyChanged();
            }
        }
    }
}