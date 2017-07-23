using System.Collections.ObjectModel;
using Captura.Models;

namespace Captura.Console
{
    class FakeWebCamProvider : NotifyPropertyChanged, IWebCamProvider
    {
        public ObservableCollection<object> AvailableCams { get; } = new ObservableCollection<object>();

        public object SelectedCam { get; set; }

        public void Dispose() { }

        public void Refresh() { }
    }
}
