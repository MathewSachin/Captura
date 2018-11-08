namespace Captura.Models
{
    public interface ISystemTray
    {
        void ShowScreenShotNotification(string FilePath);

        void HideNotification();

        void ShowNotification(INotification Notification);
    }
}
