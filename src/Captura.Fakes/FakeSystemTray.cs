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

        public void ShowScreenShotNotification(string FilePath)
        {
            // ReSharper disable once LocalizableElement
            Console.WriteLine($"{_loc.ScreenShotSaved}: {FilePath}");
        }

        public void ShowNotification(INotification Notification) { }
    }
}
