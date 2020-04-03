using System.Collections;
using System.Collections.Generic;
using Captura.FFmpeg;

namespace Captura.Fakes
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class FakeFFmpegLogRepository : IFFmpegLogRepository
    {
        public IEnumerator<IFFmpegLogEntry> GetEnumerator()
        {
            yield break;
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public IFFmpegLogEntry CreateNew(string Name, string Args)
        {
            return new FFmpegLogItem(Name, Args);
        }

        public void Remove(IFFmpegLogEntry Entry)
        {
        }
    }
}