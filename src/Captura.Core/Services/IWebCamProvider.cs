using System;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Captura.Models
{
    public interface IWebCamProvider : INotifyPropertyChanged, IDisposable
    {
        ObservableCollection<IWebcamItem> AvailableCams { get; }

        IWebcamItem SelectedCam { get; set; }

        void Refresh();
    }
}
