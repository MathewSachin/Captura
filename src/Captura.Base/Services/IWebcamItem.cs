namespace Captura.Models
{
    public interface IWebcamItem
    {
        string Name { get; }

        IWebcamCapture BeginCapture();
    }
}