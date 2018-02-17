namespace Captura.Models
{
    public interface IMessageProvider
    {
        void ShowError(string Message, string Header = null);

        bool ShowYesNo(string Message, string Title);

        void ShowFFMpegUnavailable();
    }
}
