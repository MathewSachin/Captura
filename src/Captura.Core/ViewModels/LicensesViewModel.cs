using System.Collections.Generic;
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
                Licenses = Directory.EnumerateFiles(folder).Select(FileName => new LicenseItem(FileName));
            }
        }

        public IEnumerable<LicenseItem> Licenses { get; }
    }
}