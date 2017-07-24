using Captura.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Captura
{
    public class ManageSessionViewModel : ViewModelBase
    {
        public ObservableCollection<CultureInfo> Items { get; }
    }
}
