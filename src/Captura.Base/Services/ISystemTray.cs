namespace Captura.Models
{
    public interface ISystemTray
    {
        void ShowScreenShotNotification(string FilePath);

        void HideNotification();

        INotification ShowNotification(bool Progress);
    }
}
