using System.Collections.ObjectModel;
using System.Drawing;

namespace Captura.Models
{
    class FakeWebCamProvider : NotifyPropertyChanged, IWebCamProvider
    {
        public ReadOnlyObservableCollection<IWebcamItem> AvailableCams { get; } = new ReadOnlyObservableCollection<IWebcamItem>(new ObservableCollection<IWebcamItem>());

        public IWebcamItem SelectedCam { get; set; }

        public void Dispose() { }

        public void Refresh() { }

        public Bitmap Capture() => null;
    }
}
