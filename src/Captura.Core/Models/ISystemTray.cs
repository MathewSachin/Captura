using System;

namespace Captura.Models
{
    public interface ISystemTray
    {
        void ShowTextNotification(string Text, int Duration, Action OnClick);

        void ShowScreenShotNotification(string FilePath);

        void HideNotification();
    }
}
