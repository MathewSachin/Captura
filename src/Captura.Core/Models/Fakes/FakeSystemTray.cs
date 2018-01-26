using System;
using Captura.Models;

namespace Captura.Core
{
    class FakeSystemTray : ISystemTray
    {
        public void HideNotification() { }

        public void ShowScreenShotNotification(string FilePath)
        {
            // ReSharper disable once LocalizableElement
            Console.WriteLine($"{LanguageManager.Instance.ScreenShotSaved}: {FilePath}");
        }

        public void ShowTextNotification(string Text, int Duration, Action OnClick)
        {
            Console.WriteLine(Text);
        }
    }
}
