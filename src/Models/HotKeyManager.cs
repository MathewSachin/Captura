using System.Collections.Generic;
using System.Windows.Forms;
using System.Windows.Interop;

namespace Captura
{
    static class HotKeyManager
    {
        public static readonly List<Hotkey> Hotkeys = new List<Hotkey>();
        
        public static void RegisterAll()
        {
            Hotkeys.Add(new Hotkey("Start/Stop Recording", Modifiers.Ctrl | Modifiers.Alt | Modifiers.Shift, Keys.R, () =>
            {
                var command = MainViewModel.Instance.RecordCommand;

                if (command.CanExecute(null))
                    command.Execute(null);
            }));
            
            Hotkeys.Add(new Hotkey("Pause/Resume Recording", Modifiers.Ctrl | Modifiers.Alt | Modifiers.Shift, Keys.P, () =>
            {
                var command = MainViewModel.Instance.PauseCommand;

                if (command.CanExecute(null))
                    command.Execute(null);
            }));
            
            Hotkeys.Add(new Hotkey("ScreenShot", Modifiers.Ctrl | Modifiers.Alt | Modifiers.Shift, Keys.S, () =>
            {
                var command = MainViewModel.Instance.ScreenShotCommand;

                if (command.CanExecute(null))
                    command.Execute(null);
            }));

            ComponentDispatcher.ThreadPreprocessMessage += ProcessMessage;
        }

        const int WmHotkey = 786;

        static void ProcessMessage(ref MSG Message, ref bool Handled)
        {
            if (Message.message == WmHotkey)
            {
                var id = Message.wParam.ToInt32();

                foreach (var hotkey in Hotkeys)
                {
                    if (hotkey.ID == id)
                    {
                        hotkey.Work();
                        break;
                    }
                }
            }
        }
        
        public static void UnRegisterAll()
        {
            foreach (var hotkey in Hotkeys)
                hotkey.Unregister();
        }
    }
}