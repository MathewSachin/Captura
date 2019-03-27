using System.IO;
using System.Linq;
using Captura.Models;

namespace Captura.ViewModels
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class LicensesViewModel : NotifyPropertyChanged
    {
        public LicensesViewModel()
        {
            var folder = Path.Combine(ServiceProvider.AppDir, "licenses");

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
            set => Set(ref _selectedLicense, value);
        }
    }
}