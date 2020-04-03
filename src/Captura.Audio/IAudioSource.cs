using System;
using System.Collections.Generic;

namespace Captura.Audio
{
    public interface IAudioSource : IDisposable
    {
        string Name { get; }

        IEnumerable<IAudioItem> Microphones { get; }

        IAudioItem DefaultMicrophone { get; }

        IEnumerable<IAudioItem> Speakers { get; }

        IAudioItem DefaultSpeaker { get; }

        IAudioProvider GetAudioProvider(IAudioItem Microphone, IAudioItem Speaker);

        event Action DevicesUpdated;
    }
}