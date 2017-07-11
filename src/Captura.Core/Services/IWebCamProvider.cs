using System;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Captura.Models
{
    public interface IWebCamProvider : INotifyPropertyChanged, IDisposable
    {
        ObservableCollection<object> AvailableCams { get; }

        object SelectedCam { get; set; }

        void Refresh();
    }
}
