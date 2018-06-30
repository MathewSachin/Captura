using System.ComponentModel;

namespace Captura.Models
{
    public interface IAudioItem : INotifyPropertyChanged
    {
        string Name { get; }

        bool Active { get; set; }
    }
}
