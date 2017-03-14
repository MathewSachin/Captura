using System.Collections.Generic;
using System.Linq;
using System.Windows.Interop;
using Captura.Properties;

namespace Captura
{
    static class HotKeyManager
    {
        public static readonly List<Hotkey> Hotkeys = new List<Hotkey>();
        
        public static void RegisterAll()
        {
            Hotkeys.Add(new Hotkey("Start/Stop Recording", (Modifiers) Settings.Default.RecordHotkeyMod, Settings.Default.RecordHotkey,
                () => MainViewModel.Instance.RecordCommand.ExecuteIfCan()));
            
            Hotkeys.Add(new Hotkey("Pause/Resume Recording", (Modifiers) Settings.Default.PauseHotkeyMod, Settings.Default.PauseHotkey,
                () => MainViewModel.Instance.PauseCommand.ExecuteIfCan()));
            
            Hotkeys.Add(new Hotkey("ScreenShot", (Modifiers) Settings.Default.ScreenShotHotkeyMod, Settings.Default.ScreenShotHotkey,
                () => MainViewModel.Instance.ScreenShotCommand.ExecuteIfCan()));

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

            // Save Hotkey configurations
            Settings.Default.RecordHotkey = Hotkeys[0].Key;
            Settings.Default.RecordHotkeyMod = (int) Hotkeys[0].Modifiers;

            Settings.Default.PauseHotkey = Hotkeys[1].Key;
            Settings.Default.PauseHotkeyMod = (int) Hotkeys[1].Modifiers;

            Settings.Default.ScreenShotHotkey = Hotkeys[2].Key;
            Settings.Default.ScreenShotHotkeyMod = (int) Hotkeys[2].Modifiers;
        }
    }
}