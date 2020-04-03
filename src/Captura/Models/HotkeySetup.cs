using System.Linq;
using Captura.Models;

namespace Captura.Hotkeys
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class HotkeySetup
    {
        readonly HotKeyManager _hotKeyManager;
        readonly IMessageProvider _messageProvider;

        public HotkeySetup(HotKeyManager HotKeyManager,
            IMessageProvider MessageProvider)
        {
            _hotKeyManager = HotKeyManager;
            _messageProvider = MessageProvider;
        }

        public void Setup()
        {
            _hotKeyManager.RegisterAll();
        }

        public void ShowUnregistered()
        {
            var notRegisteredOnStartup = _hotKeyManager
                .Hotkeys
                .Where(M => M.IsActive && !M.IsRegistered)
                .ToArray();

            if (notRegisteredOnStartup.Length <= 0)
                return;

            var message = "The following Hotkeys could not be registered:\nOther programs might be using them.\nTry changing them.\n\n";

            foreach (var hotkey in notRegisteredOnStartup)
            {
                message += $"{hotkey.Service.Description} - {hotkey}\n\n";
            }

            _messageProvider.ShowError(message, "Failed to Register Hotkeys");
        }
    }
}