using Captura.Models;
using Captura.ViewModels;

namespace Captura
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class HotkeySetup
    {
        readonly HotkeyActionRegisterer _hotkeyActionRegisterer;
        readonly HotKeyManager _hotKeyManager;

        public HotkeySetup(HotkeyActionRegisterer HotkeyActionRegisterer,
            HotKeyManager HotKeyManager)
        {
            _hotkeyActionRegisterer = HotkeyActionRegisterer;
            _hotKeyManager = HotKeyManager;
        }

        public void Setup()
        {
            _hotKeyManager.RegisterAll();

            _hotkeyActionRegisterer.Register();

            var listener = new HotkeyListener();

            listener.HotkeyReceived += Id => _hotKeyManager.ProcessHotkey(Id);

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
            _hotKeyManager.ShowNotRegisteredOnStartup();
        }
    }
}