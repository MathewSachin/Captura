using System.Collections.ObjectModel;
using System.ComponentModel;
using Captura.Models;

namespace Captura.Console
{
    class FakeWebCamProvider : IWebCamProvider
    {
        public ObservableCollection<object> AvailableCams { get; }

        public object SelectedCam { get; set; }
        
        public event PropertyChangedEventHandler PropertyChanged;

        public void Dispose() { }

        public void Refresh() { }
    }
}
