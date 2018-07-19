using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Captura.Models
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class KeymapViewModel : NotifyPropertyChanged
    {
        Keymap _keymap;
        readonly string _keymapDir;

        public const string DefaultKeymapFileName = "en";

        public class KeymapItem
        {
            public KeymapItem(string FileName, string Name)
            {
                this.FileName = FileName;
                this.Name = Name;
            }

            public string FileName { get; }

            public string Name { get; }
        }

        public KeymapViewModel()
        {
            _keymapDir = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "keymaps");

            if (Directory.Exists(_keymapDir))
            {
                var files = Directory.EnumerateFiles(_keymapDir, "*.json");

                AvailableKeymaps = files
                    .Where(M => !M.Contains("schema"))
                    .Select(M =>
                    {
                        var content = File.ReadAllText(M);

                        var name = JObject.Parse(content)["Name"];

                        return new KeymapItem(M, name.ToString());
                    }).ToList();

                if (AvailableKeymaps.Count > 0)
                    SelectedKeymap = AvailableKeymaps[0];
            }
        }

        public IReadOnlyList<KeymapItem> AvailableKeymaps { get; }

        KeymapItem _selectedKeymap;

        public KeymapItem SelectedKeymap
        {
            get => _selectedKeymap;
            set
            {
                if (!File.Exists(value.FileName))
                    return;

                try
                {
                    Parse(File.ReadAllText(value.FileName));

                    _selectedKeymap = value;

                    OnPropertyChanged();
                }
                catch
                {
                    // Ignore errors
                }
            }
        }

        void Init(Keymap Keymap)
        {
            _keymap = Keymap;

            Control = Find(Keys.Control, ModifierStates.Empty) ?? nameof(Control);
            Shift = Find(Keys.Shift, ModifierStates.Empty) ?? nameof(Shift);
            Alt = Find(Keys.Alt, ModifierStates.Empty) ?? nameof(Alt);
        }

        void Parse(string Content)
        {
            var keymap = new Keymap();

            JsonConvert.PopulateObject(Content, keymap);
            
            Init(keymap);
        }

        public string Find(Keys Key, ModifierStates Modifiers)
        {
            return _keymap.Mappings
                .Where(M => M.On.Any(S => S.Control == Modifiers.Control
                              && S.Alt == Modifiers.Alt
                              && S.Shift == Modifiers.Shift
                              && S.CapsLock == Modifiers.CapsLock))
                .SelectMany(M => M.Keys)
                .FirstOrDefault(M => M.Key == Key).Value;
        }

        public string Control { get; private set; } = nameof(Control);

        public string Shift { get; private set; } = nameof(Shift);

        public string Alt { get; private set; } = nameof(Alt);
    }
}