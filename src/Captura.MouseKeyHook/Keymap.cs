// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
// ReSharper disable MemberCanBePrivate.Global

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace Captura.Models
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class Keymap : NotifyPropertyChanged
    {
        List<KeymapItem> _parsed;
        List<KeymapItem> _capsLocked;
        readonly string _keymapDir;

        public const string DefaultKeymapFileName = "English";

        public Keymap()
        {
            _keymapDir = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "keymaps");

            if (Directory.Exists(_keymapDir))
            {
                var files = Directory.EnumerateFiles(_keymapDir, "*.dat");

                AvailableKeymaps = files.Select(Path.GetFileNameWithoutExtension);

                SelectedKeymap = DefaultKeymapFileName;
            }
        }

        public IEnumerable<string> AvailableKeymaps { get; }

        string _selectedKeymap;

        public string SelectedKeymap
        {
            get => _selectedKeymap;
            set
            {
                var filePath = Path.Combine(_keymapDir, value + ".dat");

                if (!File.Exists(filePath))
                    return;

                _selectedKeymap = value;

                var lines = File.ReadAllLines(filePath);

                Parse(lines);

                OnPropertyChanged();
            }
        }

        void Init(List<KeymapItem> Parsed, List<KeymapItem> CapsLocked)
        {
            _parsed = Parsed;
            _capsLocked = CapsLocked;

            Control = Find(Keys.Control) ?? nameof(Control);
            Shift = Find(Keys.Shift) ?? nameof(Shift);
            Alt = Find(Keys.Alt) ?? nameof(Alt);
        }

        class KeymapItem
        {
            public KeymapItem(Keys Keys, string Value)
            {
                this.Keys = Keys;
                this.Value = Value;
            }

            public Keys Keys { get; }

            public string Value { get; }
        }

        public void Parse(string[] Lines)
        {
            var parsed = new List<KeymapItem>();
            var capsLocked = new List<KeymapItem>();

            foreach (var line in Lines)
            {
                try
                {
                    // Comment or Blank
                    if (string.IsNullOrWhiteSpace(line) || line.StartsWith("//"))
                        continue;

                    var split = line
                        .Split("=".ToCharArray(), 2, StringSplitOptions.RemoveEmptyEntries);

                    if (split.Length != 2)
                        throw new FormatException();

                    var capsLock = split[0][split[0].Length - 1] == '^';

                    if (capsLock)
                        split[0] = split[0].Substring(0, split[0].Length - 1);

                    var keys = split[0].Split("+".ToCharArray(), StringSplitOptions.RemoveEmptyEntries)
                        .Select(M => M.Trim())
                        .Select(M => (Keys) Enum.Parse(typeof(Keys), M, true))
                        .Aggregate((K1, K2) => K1 | K2);

                    var val = new KeymapItem(keys, split[1].Trim());

                    if (capsLock)
                        capsLocked.Add(val);
                    else parsed.Add(val);
                }
                catch
                {
                    // Ignore errors
                }
            }

            Init(parsed, capsLocked);
        }

        public string Find(Keys Keys, bool CapsLock = false)
        {
            if (CapsLock)
            {
                var match = _capsLocked.Find(M => M.Keys == Keys);

                if (match != null)
                    return match.Value;
            }

            return _parsed.Find(M => M.Keys == Keys)?.Value;
        }

        public string Control { get; private set; } = nameof(Control);

        public string Shift { get; private set; } = nameof(Shift);

        public string Alt { get; private set; } = nameof(Alt);
    }
}