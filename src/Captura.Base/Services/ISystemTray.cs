using System;

namespace Captura.Models
{
    public interface ISystemTray
    {
        void ShowTextNotification(string Text, Action OnClick);

        void ShowScreenShotNotification(string FilePath);

        void HideNotification();

        void ShowMessage(string Message);

        void ShowError(string Error);

        ITrayProgress ShowProgress();
    }
}
