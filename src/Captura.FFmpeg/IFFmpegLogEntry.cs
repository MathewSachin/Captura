namespace Captura.Models
{
    public interface IFFmpegLogEntry
    {
        void Write(string Line);

        string GetCompleteLog();
    }
}