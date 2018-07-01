using System;

namespace Captura.Models
{
    // ReSharper disable once ClassNeverInstantiated.Global
    class FakeSystemTray : ISystemTray
    {
        readonly LanguageManager _loc;

        public FakeSystemTray(LanguageManager Loc)
        {
            _loc = Loc;
        }

        public void HideNotification() { }

        public void ShowMessage(string Message)
        {
            Console.WriteLine(Message);
        }

        public void ShowError(string Error)
        {
            Console.WriteLine(Error);
        }

        public void ShowTextNotification(string Text, Action OnClick)
        {
            Console.WriteLine(Text);
        }

        public void ShowScreenShotNotification(string FilePath)
        {
            // ReSharper disable once LocalizableElement
            Console.WriteLine($"{_loc.ScreenShotSaved}: {FilePath}");
        }

        public ITrayProgress ShowProgress() => new TrayProgressViewModel();
    }
}
