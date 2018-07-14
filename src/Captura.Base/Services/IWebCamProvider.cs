using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;

namespace Captura.Models
{
    public interface IWebCamProvider : INotifyPropertyChanged
    {
        ReadOnlyObservableCollection<IWebcamItem> AvailableCams { get; }

        IWebcamItem SelectedCam { get; set; }

        void Refresh();

        Bitmap Capture();

        int Width { get; }

        int Height { get; }
    }
}
