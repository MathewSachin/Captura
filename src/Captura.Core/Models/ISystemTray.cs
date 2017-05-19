using System;

namespace Captura.Models
{
    interface ISystemTray
    {
        void ShowTextNotification(string Text, string Title, int Duration, Action OnClick = null);

        void ShowScreenShotNotification(string FilePath);

        void HideNotification();
    }
}
