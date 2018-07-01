using System;

namespace Captura.Models
{
    class FakeSystemTray : ISystemTray
    {
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
            Console.WriteLine($"{LanguageManager.Instance.ScreenShotSaved}: {FilePath}");
        }

        public ITrayProgress ShowProgress() => new TrayProgressViewModel();
    }
}
