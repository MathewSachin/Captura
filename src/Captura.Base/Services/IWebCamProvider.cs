using System;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Captura.Models
{
    public interface IWebCamProvider : INotifyPropertyChanged
    {
        ReadOnlyObservableCollection<IWebcamItem> AvailableCams { get; }

        IWebcamItem SelectedCam { get; set; }

        void Refresh();

        IDisposable Capture(IBitmapLoader BitmapLoader);

        int Width { get; }

        int Height { get; }
    }
}
