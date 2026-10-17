using System;
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
                var input = new BufferedWaveProvider(provider.NAudioWaveFormat)
                {
                    DiscardOnBufferOverflow = true,
                    ReadFully = false
                };

                var sampleProvider = input.ToSampleProvider();

                var providerWf = provider.WaveFormat;

                // Mono to Stereo
                if (providerWf.Channels == 1)
                    sampleProvider = sampleProvider.ToStereo();

                // Resample
                var resample = providerWf.SampleRate != WaveFormat.SampleRate;

                if (resample)
                {
                    sampleProvider = new WdlResamplingSampleProvider(sampleProvider, WaveFormat.SampleRate);
                }

                var channels = sampleProvider.WaveFormat.Channels;

                var output = new BufferedWaveProvider(NAudio.Wave.WaveFormat.CreateIeeeFloatWaveFormat(WaveFormat.SampleRate, channels))
                {
                    DiscardOnBufferOverflow = true,
                    ReadFully = false
                };

                var ratio = (double)providerWf.SampleRate / WaveFormat.SampleRate;
                var chunkFrames = WaveFormat.SampleRate / 10;
                var samples = new float[chunkFrames * channels];
                var bytes = new byte[chunkFrames * channels * 4];

                provider.WaveIn.DataAvailable += (S, E) =>
                {
                    input.AddSamples(E.Buffer, 0, E.BytesRecorded);

                    while (true)
                    {
                        var available = input.BufferedBytes / input.WaveFormat.BlockAlign;

                        // WdlResamplingSampleProvider flushes if its source returns fewer samples than asked,
                        // causing clicks and lost samples when upsampling, so ask only for what is buffered.
                        var frames = resample
                            ? Math.Min((int)((available - 4) / ratio), chunkFrames)
                            : Math.Min(available, chunkFrames);

                        if (frames <= 0)
                            break;

                        frames = sampleProvider.Read(samples, 0, frames * channels) / channels;

                        if (frames == 0)
                            break;

                        Buffer.BlockCopy(samples, 0, bytes, 0, frames * channels * 4);
                        output.AddSamples(bytes, 0, frames * channels * 4);
                    }
                };

                _audioProviders.Add(provider, output.ToSampleProvider());
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