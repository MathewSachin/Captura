using System;
using System.Collections.Generic;
using Captura.Audio;

namespace Captura.NAudio
{
    // TODO: Make this work ;-(
    class MixedAudioProvider : IAudioProvider
    {
        readonly IEnumerable<NAudioProvider> _audioProviders;

        public MixedAudioProvider(IEnumerable<NAudioProvider> AudioProviders)
        {
            _audioProviders = AudioProviders;
        }

        public void Dispose()
        {
            foreach (var provider in _audioProviders)
            {
                provider.Dispose();
            }
        }

        public WaveFormat WaveFormat { get; } = WaveFormat.CreateIeeeFloatWaveFormat(44100, 2);

        public void Start()
        {
            foreach (var provider in _audioProviders)
            {
                provider.Start();
            }
        }

        public void Stop()
        {
            foreach (var provider in _audioProviders)
            {
                provider.Stop();
            }
        }

        public event EventHandler<DataAvailableEventArgs> DataAvailable;
    }
}