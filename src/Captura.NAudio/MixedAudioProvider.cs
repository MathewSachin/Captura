using System;
using System.Collections.Generic;
using Captura.Audio;
using NAudio.Wave;
using WaveFormat = Captura.Audio.WaveFormat;
using Wf = NAudio.Wave.WaveFormat;

namespace Captura.NAudio
{
    // TODO: Make this work ;-(
    // TODO: Implement changing audio device active state
    class MixedAudioProvider : IAudioProvider
    {
        readonly Dictionary<NAudioProvider, ISampleProvider> _audioProviders = new Dictionary<NAudioProvider, ISampleProvider>();

        public MixedAudioProvider(IEnumerable<NAudioProvider> AudioProviders)
        {
            foreach (var provider in AudioProviders)
            {
                var bufferedProvider = new BufferedWaveProvider(Wf.CreateIeeeFloatWaveFormat(44100, 2));

                provider.DataAvailable += (S, E) =>
                {
                    bufferedProvider.AddSamples(E.Buffer, 0, E.Length);
                };

                var sampleProvider = bufferedProvider.ToSampleProvider();

                _audioProviders.Add(provider, sampleProvider);
            }
        }

        public void Dispose()
        {
            foreach (var provider in _audioProviders.Keys)
            {
                provider.Dispose();
            }
        }

        public WaveFormat WaveFormat { get; } = WaveFormat.CreateIeeeFloatWaveFormat(44100, 2);

        public void Start()
        {
            foreach (var provider in _audioProviders.Keys)
            {
                provider.Start();
            }
        }

        public void Stop()
        {
            foreach (var provider in _audioProviders.Keys)
            {
                provider.Stop();
            }
        }

        public event EventHandler<DataAvailableEventArgs> DataAvailable;
    }
}