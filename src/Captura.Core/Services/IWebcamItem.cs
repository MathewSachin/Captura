using System.ComponentModel;

namespace Captura.Models
{
    public interface IWebcamItem : INotifyPropertyChanged
    {
        string Name { get; }
    }
}