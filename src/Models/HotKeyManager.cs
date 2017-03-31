using System.Collections.Generic;
using System.Linq;
using System.Windows.Interop;

namespace Captura
{
    static class HotKeyManager
    {
        public static readonly List<PersistedHotkey> Hotkeys = new List<PersistedHotkey>();
        
        public static void RegisterAll()
        {
            Hotkeys.Add(new PersistedHotkey("Start/Stop Recording", nameof(Settings.RecordHotkeyMod), nameof(Settings.RecordHotkey),
                () => MainViewModel.Instance.RecordCommand.ExecuteIfCan()));
            
            Hotkeys.Add(new PersistedHotkey("Pause/Resume Recording", nameof(Settings.PauseHotkeyMod), nameof(Settings.PauseHotkey),
                () => MainViewModel.Instance.PauseCommand.ExecuteIfCan()));
            
            Hotkeys.Add(new PersistedHotkey("ScreenShot", nameof(Settings.ScreenShotHotkeyMod), nameof(Settings.ScreenShotHotkey),
                () => MainViewModel.Instance.ScreenShotCommand.ExecuteIfCan()));

            Hotkeys.Add(new PersistedHotkey("ScreenShot Active Window", nameof(Settings.ActiveScreenShotHotkeyMod), nameof(Settings.ActiveScreenShotHotkey),
                () => MainViewModel.Instance.SaveScreenShot(MainViewModel.Instance.ScreenShotWindow(Screna.Window.ForegroundWindow))));
            
            Hotkeys.Add(new PersistedHotkey("ScreenShot Desktop", nameof(Settings.DesktopHotkeyMod), nameof(Settings.DesktopHotkey),
                () => MainViewModel.Instance.SaveScreenShot(MainViewModel.Instance.ScreenShotWindow(Screna.Window.DesktopWindow))));

            // Register for Windows Messages
            ComponentDispatcher.ThreadPreprocessMessage += ProcessMessage;
        }

        const int WmHotkey = 786;

        static void ProcessMessage(ref MSG Message, ref bool Handled)
        {
            // Is Hotkey Message
            if (Message.message == WmHotkey)
            {
                var id = Message.wParam.ToInt32();

                Hotkeys.SingleOrDefault(h => h.ID == id)?.Work();                
            }
        }
        
        public static void Dispose()
        {
            // Unregister All Hotkeys
            Hotkeys.ForEach(h => h.Unregister());
        }
    }
}