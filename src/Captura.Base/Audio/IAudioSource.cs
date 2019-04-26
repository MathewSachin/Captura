using Captura.Audio;
using System;
using System.Collections.Generic;

namespace Captura.Models
{
    public interface IAudioSource : IDisposable
    {
        string Name { get; }

        IEnumerable<IAudioItem> GetSources();

        IAudioProvider GetMixedAudioProvider(IEnumerable<IIsActive<IAudioItem>> AudioItems);

        IAudioProvider GetAudioProvider(IAudioItem AudioItem);

        bool CanChangeSourcesDuringRecording { get; }
    }
}