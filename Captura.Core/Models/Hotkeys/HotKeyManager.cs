using Captura.Models;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Captura
{
    public static class HotKeyManager
    {
        public static readonly List<Hotkey> Hotkeys = new List<Hotkey>();
        
        public static void RegisterAll()
        {
            InitStorage();

            foreach (var model in Settings.Instance.Hotkeys)
                Hotkeys.Add(new Hotkey(model));

            ServiceProvider.HotKeyPressed += ProcessHotkey;
        }

        static void InitStorage()
        {
            if (Settings.Instance.Hotkeys == null)
                Settings.Instance.Hotkeys = new List<HotkeyModel>();

            if (Settings.Instance.Hotkeys.Count == 0)
                Reset();
        }

        public static void Reset()
        {
            Settings.Instance.Hotkeys.Clear();

            Settings.Instance.Hotkeys.AddRange(new[]
            {
                new HotkeyModel(ServiceName.Recording, Keys.R, Modifiers.Alt | Modifiers.Ctrl | Modifiers.Shift),
                new HotkeyModel(ServiceName.Pause, Keys.P, Modifiers.Alt | Modifiers.Ctrl | Modifiers.Shift),
                new HotkeyModel(ServiceName.ScreenShot, Keys.S, Modifiers.Alt | Modifiers.Ctrl | Modifiers.Shift),
                new HotkeyModel(ServiceName.ActiveScreenShot, Keys.PrintScreen, Modifiers.Alt),
                new HotkeyModel(ServiceName.DesktopScreenShot, Keys.D, Modifiers.Alt | Modifiers.Ctrl | Modifiers.Shift)
            });
        }
        
        static void ProcessHotkey(int Id)
        {
            Hotkeys.SingleOrDefault(h => h.ID == Id)?.Work();                
        }
        
        public static void Dispose()
        {
            Settings.Instance.Hotkeys.Clear();
                        
            Hotkeys.ForEach(h => 
            {
                // Unregister All Hotkeys
                h.Unregister();

                // Save
                Settings.Instance.Hotkeys.Add(new HotkeyModel(h.ServiceName, h.Key, h.Modifiers));
            });
        }
    }
}