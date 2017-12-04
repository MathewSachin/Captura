using System;
using Captura.Models;

namespace Captura.Console
{
    class FakeSystemTray : ISystemTray
    {
        public void HideNotification() { }

        public void ShowScreenShotNotification(string FilePath)
        {
            // ReSharper disable once LocalizableElement
            System.Console.WriteLine($"{LanguageManager.ScreenShotSaved}: {FilePath}");
        }

        public void ShowTextNotification(string Text, int Duration, Action OnClick)
        {
            System.Console.WriteLine(Text);
        }
    }
}
