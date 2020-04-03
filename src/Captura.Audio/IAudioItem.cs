using System;

namespace Captura.Audio
{
    public interface IAudioItem : IDisposable
    {
        string Name { get; }

        bool IsLoopback { get; }

        void StartListeningForPeakLevel();

        void StopListeningForPeakLevel();

        double PeakLevel { get; }
    }
}
