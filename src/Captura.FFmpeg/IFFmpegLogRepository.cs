using System.Collections.Generic;

namespace Captura.Models
{
    public interface IFFmpegLogRepository : IEnumerable<IFFmpegLogEntry>
    {
        IFFmpegLogEntry CreateNew(string Name, string Args);

        void Remove(IFFmpegLogEntry Entry);
    }
}