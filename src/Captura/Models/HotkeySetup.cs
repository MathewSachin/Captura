using System.Linq;
using Captura.Models;
using Captura.ViewModels;

namespace Captura
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class HotkeySetup
    {
        readonly HotkeyActionRegisterer _hotkeyActionRegisterer;
        readonly HotKeyManager _hotKeyManager;
        readonly IMessageProvider _messageProvider;

        public HotkeySetup(HotkeyActionRegisterer HotkeyActionRegisterer,
            HotKeyManager HotKeyManager,
            IMessageProvider MessageProvider)
        {
            _hotkeyActionRegisterer = HotkeyActionRegisterer;
            _hotKeyManager = HotKeyManager;
            _messageProvider = MessageProvider;
        }

        public void Setup()
        {
            _hotKeyManager.RegisterAll();

            _hotkeyActionRegisterer.Register();

            _hotKeyManager.HotkeyPressed += Service =>
            {
                switch (Service)
                {
                    case ServiceName.OpenImageEditor:
                        new ImageEditorWindow().ShowAndFocus();
                        break;

                    case ServiceName.ShowMainWindow:
                        MainWindow.Instance.ShowAndFocus();
                        break;
                }
            };
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