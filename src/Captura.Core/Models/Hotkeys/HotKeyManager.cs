using Captura.Models;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Captura
{
    public static class HotKeyManager
    {
        public static readonly List<Hotkey> Hotkeys = new List<Hotkey>();

        static HotKeyManager()
        {
            ServiceProvider.HotKeyPressed += ProcessHotkey;
        }

        public static void RegisterAll()
        {
            Populate();
        }

        public static void Populate()
        {
            if (Settings.Instance.Hotkeys == null)
                Settings.Instance.Hotkeys = new List<HotkeyModel>();

            if (Settings.Instance.Hotkeys.Count == 0)
                Reset();

            var nonReg = new List<Hotkey>();

            foreach (var model in Settings.Instance.Hotkeys)
            {
                var hotkey = new Hotkey(model);

                if (!hotkey.IsRegistered)
                    nonReg.Add(hotkey);

                Hotkeys.Add(hotkey);
            }

            if (nonReg.Count > 0)
            {
                var message = "The following Hotkeys could not be registered:\nOther programs might be using them.\nTry changing them.\n\n";

                foreach (var hotkey in nonReg)
                {
                    message += $"{TranslationSource.Instance[ServiceProvider.GetDescriptionKey(hotkey.ServiceName)]} - {hotkey}\n\n";
                }

                ServiceProvider.MessageProvider.ShowError(message);
            }
        }

        public static void Reset()
        {
            Settings.Instance.Hotkeys.Clear();

            Settings.Instance.Hotkeys.AddRange(new[]
            {
                new HotkeyModel(ServiceName.Recording, Keys.R, Modifiers.Alt | Modifiers.Ctrl | Modifiers.Shift, true),
                new HotkeyModel(ServiceName.Pause, Keys.P, Modifiers.Alt | Modifiers.Ctrl | Modifiers.Shift, true),
                new HotkeyModel(ServiceName.ScreenShot, Keys.S, Modifiers.Alt | Modifiers.Ctrl | Modifiers.Shift, true),
                new HotkeyModel(ServiceName.ActiveScreenShot, Keys.PrintScreen, Modifiers.Alt, true),
                new HotkeyModel(ServiceName.DesktopScreenShot, Keys.D, Modifiers.Alt | Modifiers.Ctrl | Modifiers.Shift, true)
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
                Settings.Instance.Hotkeys.Add(new HotkeyModel(h.ServiceName, h.Key, h.Modifiers, h.IsActive));
            });
        }
    }
}