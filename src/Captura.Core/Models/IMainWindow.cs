namespace Captura.Models
{
    public interface IMainWindow
    {
        bool IsVisible { get; set; }

        bool IsMinimized { get; set; }
    }
}
