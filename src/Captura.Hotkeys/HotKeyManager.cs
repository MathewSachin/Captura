using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace Captura.Hotkeys
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class HotKeyManager : IDisposable
    {
        readonly ObservableCollection<Hotkey> _hotkeys = new ObservableCollection<Hotkey>();

        public ReadOnlyObservableCollection<Hotkey> Hotkeys { get; }

        static string GetFilePath() => Path.Combine(ServiceProvider.SettingsDir, "Hotkeys.json");

        public HotKeyManager(IHotkeyListener HotkeyListener,
            IEnumerable<IHotkeyActor> HotkeyActors)
        {
            Hotkeys = new ReadOnlyObservableCollection<Hotkey>(_hotkeys);

            HotkeyListener.HotkeyReceived += ProcessHotkey;
            HotkeyPressed += M =>
            {
                foreach (var actor in HotkeyActors)
                {
                    actor.Act(M);
                }
            };
        }

        public void Remove(Hotkey Hotkey)
        {
            Hotkey.Unregister();

            _hotkeys.Remove(Hotkey);
        }

        public void Add()
        {
            var hotkey = new Hotkey(new HotkeyModel(ServiceName.None, Keys.None, Modifiers.None, false));

            _hotkeys.Add(hotkey);
        }

        public static IEnumerable<Service> AllServices { get; } = Enum
            .GetValues(typeof(ServiceName))
            .Cast<ServiceName>()
            .Select(M => new Service(M))
            .ToArray(); // Prevent multiple enumerations

        public void RegisterAll()
        {
            IEnumerable<HotkeyModel> models;

            try
            {
                var json = File.ReadAllText(GetFilePath());

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

            _hotkeys.Clear();

            Populate(Defaults());
        }

        void Populate(IEnumerable<HotkeyModel> Models)
        {
            foreach (var model in Models)
            {
                var hotkey = new Hotkey(model);

                _hotkeys.Add(hotkey);
            }
        }

        static IEnumerable<HotkeyModel> Defaults()
        {
            return new[]
            {
                new HotkeyModel(ServiceName.Recording, Keys.F9, Modifiers.Alt, true),
                new HotkeyModel(ServiceName.Pause, Keys.F9, Modifiers.Shift, true),
                new HotkeyModel(ServiceName.ScreenShot, Keys.PrintScreen, 0, true),
                new HotkeyModel(ServiceName.ActiveScreenShot, Keys.PrintScreen, Modifiers.Alt, true),
                new HotkeyModel(ServiceName.DesktopScreenShot, Keys.PrintScreen, Modifiers.Shift, true)
            };
        }
        
        void ProcessHotkey(int Id)
        {
            var hotkey = Hotkeys.SingleOrDefault(H => H.Id == Id);

            if (hotkey != null)
                HotkeyPressed?.Invoke(hotkey.Service.ServiceName);
        }

        public void FakeHotkey(ServiceName Service)
        {
            HotkeyPressed?.Invoke(Service);
        }

        event Action<ServiceName> HotkeyPressed;
        
        public void Dispose()
        {
            var models = Hotkeys.Select(M =>
            {
                M.Unregister();

                return new HotkeyModel(M.Service.ServiceName, M.Key, M.Modifiers, M.IsActive);
            });

            try
            {
                var json = JsonConvert.SerializeObject(models);

                File.WriteAllText(GetFilePath(), json);
            }
            catch
            {
                // Ignore Errors
            }
        }
    }
}