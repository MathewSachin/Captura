using System;

namespace Captura.Hotkeys
{
    public interface IHotkeyListener
    {
        event Action<int> HotkeyReceived;
    }
}