using System;

namespace Captura.Models
{
    public interface IHotkeyListener
    {
        event Action<int> HotkeyReceived;
    }
}