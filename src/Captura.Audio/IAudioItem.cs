using System;

namespace Captura.Audio
{
    public interface IAudioItem : IDisposable
    {
        string Name { get; }

        bool IsLoopback { get; }

        double PeakLevel { get; }
    }
}
