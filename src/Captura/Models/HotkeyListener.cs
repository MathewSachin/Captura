using System;
using System.Windows.Interop;

namespace Captura.Hotkeys
{
    public class HotkeyListener : IHotkeyListener
    {
        const int WindowsMessageHotkey = 786;

        public HotkeyListener()
        {
            ComponentDispatcher.ThreadPreprocessMessage += (ref MSG Message, ref bool Handled) =>
            {
                if (Message.message == WindowsMessageHotkey)
                {
                    var id = Message.wParam.ToInt32();

                    HotkeyReceived?.Invoke(id);
                }
            };
        }

        public event Action<int> HotkeyReceived;
    }
}