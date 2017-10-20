using System.Collections.ObjectModel;
using System.Drawing;
using Captura.Models;

namespace Captura.Console
{
    class FakeWebCamProvider : NotifyPropertyChanged, IWebCamProvider
    {
        public ObservableCollection<IWebcamItem> AvailableCams { get; } = new ObservableCollection<IWebcamItem>();

        public IWebcamItem SelectedCam { get; set; }

        public void Dispose() { }

        public void Refresh() { }

        public Bitmap Capture() => null;
    }
}
