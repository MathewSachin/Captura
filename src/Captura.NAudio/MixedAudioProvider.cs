using System.Collections.Generic;
using System.Linq;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;

namespace Captura.Audio
{
    class MixedAudioProvider : IAudioProvider
    {
        readonly Dictionary<NAudioProvider, ISampleProvider> _audioProviders = new Dictionary<NAudioProvider, ISampleProvider>();

        readonly IWaveProvider _mixingWaveProvider;

        public MixedAudioProvider(params NAudioProvider[] AudioProviders)
        {
            foreach (var provider in AudioProviders)
            {
                var bufferedProvider = new BufferedWaveProvider(provider.NAudioWaveFormat)
                {
                    DiscardOnBufferOverflow = true,
                    ReadFully = false
                };

                provider.WaveIn.DataAvailable += (S, E) =>
                {
                    bufferedProvider.AddSamples(E.Buffer, 0, E.BytesRecorded);
                };

                var sampleProvider = bufferedProvider.ToSampleProvider();

                var providerWf = provider.WaveFormat;

                // Mono to Stereo
                if (providerWf.Channels == 1)
                    sampleProvider = sampleProvider.ToStereo();

                // Resample
                if (providerWf.SampleRate != WaveFormat.SampleRate)
                {
                    sampleProvider = new WdlResamplingSampleProvider(sampleProvider, WaveFormat.SampleRate);
                }

                _audioProviders.Add(provider, sampleProvider);
            }

            if (_audioProviders.Count == 1)
            {
                _mixingWaveProvider = _audioProviders
                    .Values
                    .First()
                    .ToWaveProvider16();
            }
            else
            {
                var waveProviders = _audioProviders.Values.Select(M => M.ToWaveProvider());

                // MixingSampleProvider cannot be used here due to it removing inputs that don't return as many bytes as requested.

                // Screna expects 44.1 kHz 16-bit Stereo
                _mixingWaveProvider = new MixingWaveProvider32(waveProviders)
                    .ToSampleProvider()
                    .ToWaveProvider16();
            }
        }

        public void Dispose()
        {
            foreach (var provider in _audioProviders.Keys)
            {
                provider.Dispose();
            }
        }

        public WaveFormat WaveFormat { get; } = new WaveFormat();

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

        public int Read(byte[] Buffer, int Offset, int Length)
        {
            return _mixingWaveProvider.Read(Buffer, Offset, Length);
        }
    }
}