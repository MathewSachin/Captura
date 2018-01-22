using System;
using Captura.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Input;
using Newtonsoft.Json;

namespace Captura
{
    public class HotKeyManager : IDisposable
    {
        readonly ObservableCollection<Hotkey> hotkeys = new ObservableCollection<Hotkey>();

        public ReadOnlyObservableCollection<Hotkey> Hotkeys { get; }

        readonly string FilePath;

        public ICommand ResetCommand { get; }

        public HotKeyManager()
        {
            Hotkeys = new ReadOnlyObservableCollection<Hotkey>(hotkeys);

            ResetCommand = new DelegateCommand(Reset);

            ServiceProvider.HotKeyPressed += ProcessHotkey;

            FilePath = Path.Combine(ServiceProvider.SettingsDir, "Hotkeys.json");
        }

        public void RegisterAll()
        {
            IEnumerable<HotkeyModel> models;

            try
            {
                var json = File.ReadAllText(FilePath);

                models = JsonConvert.DeserializeObject<IEnumerable<HotkeyModel>>(json);
            }
            catch
            {
                models = Defaults();
            }

            Populate(models);
        }

        public void Reset()
        {
            Dispose();

            hotkeys.Clear();

            Populate(Defaults());
        }

        void Populate(IEnumerable<HotkeyModel> Models)
        {
            var nonReg = new List<Hotkey>();

            foreach (var model in Models)
            {
                var hotkey = new Hotkey(model);

                if (hotkey.IsActive && !hotkey.IsRegistered)
                    nonReg.Add(hotkey);

                hotkeys.Add(hotkey);
            }

            if (nonReg.Count > 0)
            {
                var message = "The following Hotkeys could not be registered:\nOther programs might be using them.\nTry changing them.\n\n";

                foreach (var hotkey in nonReg)
                {
                    message += $"{LanguageManager.Instance[ServiceProvider.GetDescriptionKey(hotkey.ServiceName)]} - {hotkey}\n\n";
                }

                ServiceProvider.MessageProvider.ShowError(message);
            }
        }

        IEnumerable<HotkeyModel> Defaults()
        {
            yield return new HotkeyModel(ServiceName.Recording, Keys.F9, Modifiers.Alt, true);
            yield return new HotkeyModel(ServiceName.Pause, Keys.F9, Modifiers.Shift, true);
            yield return new HotkeyModel(ServiceName.ScreenShot, Keys.PrintScreen, 0, true);
            yield return new HotkeyModel(ServiceName.ActiveScreenShot, Keys.PrintScreen, Modifiers.Alt, true);
            yield return new HotkeyModel(ServiceName.DesktopScreenShot, Keys.PrintScreen, Modifiers.Shift, true);
        }
        
        void ProcessHotkey(int Id)
        {
            Hotkeys.SingleOrDefault(H => H.ID == Id)?.Work();
        }
        
        public void Dispose()
        {
            var models = Hotkeys.Select(M =>
            {
                M.Unregister();

                return new HotkeyModel(M.ServiceName, M.Key, M.Modifiers, M.IsActive);
            });

            try
            {
                var json = JsonConvert.SerializeObject(models);

                File.WriteAllText(FilePath, json);
            }
            catch
            {
                // Ignore Errors
            }
        }
    }
}