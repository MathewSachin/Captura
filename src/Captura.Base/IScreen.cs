using System.Drawing;

namespace Captura.Models
{
    public interface IScreen
    {
        Rectangle Rectangle { get; }

        string DeviceName { get; }
    }
}