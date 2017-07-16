namespace Captura.Models
{
    public interface IMessageProvider
    {
        void ShowError(string Message);

        bool ShowYesNo(string Message, string Title);

        void ShowFFMpegUnavailable();
    }
}
