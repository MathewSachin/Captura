using System.Collections.ObjectModel;
using System.Windows.Input;
using Captura.Models;

namespace Captura.ViewModels
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class HotkeysViewModel
    {
        public ReadOnlyObservableCollection<Hotkey> Hotkeys { get; }

        public HotkeysViewModel(HotKeyManager HotKeyManager)
        {
            Hotkeys = HotKeyManager.Hotkeys;

            ResetCommand = new DelegateCommand(HotKeyManager.Reset);

            AddCommand = new DelegateCommand(HotKeyManager.Add);

            RemoveCommand = new DelegateCommand(M =>
            {
                if (M is Hotkey hotkey)
                {
                    HotKeyManager.Remove(hotkey);
                }
            });
        }

        public ICommand ResetCommand { get; }

        public ICommand AddCommand { get; }

        public ICommand RemoveCommand { get; }
    }
}