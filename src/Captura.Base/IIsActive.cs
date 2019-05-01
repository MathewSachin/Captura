using System.ComponentModel;

namespace Captura.Models
{
    public interface IIsActive<T> : INotifyPropertyChanged
    {
        T Item { get; }

        bool IsActive { get; set; }
    }
}
