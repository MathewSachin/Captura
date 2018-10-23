namespace Captura.Models
{
    public interface IDialogService
    {
        string PickFolder(string Current, string Description);

        string PickFile(string InitialFolder, string Description);
    }
}