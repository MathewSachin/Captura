using System;

namespace Captura.Models
{
    public interface IFFmpegLogEntry
    {
        void Write(string Line);

        string GetCompleteLog();

        event Action<int> ProgressChanged;
    }
}