using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;

namespace Captura.Audio
{
    class MixedAudioProvider : IAudioProvider
    {
        readonly Dictionary<NAudioProvider, ISampleProvider> _audioProviders = new Dictionary<NAudioProvider, ISampleProvider>();

        readonly IWaveProvider _mixingWaveProvider;

        readonly ManualResetEvent _continueEvent = new ManualResetEvent(false),
            _stopEvent = new ManualResetEvent(false);

        byte[] _buffer;
        const int ReadInterval = 200;

        public MixedAudioProvider(params NAudioProvider[] AudioProviders)
        {
            foreach (var provider in AudioProviders)
            {
                var bufferedProvider = new BufferedWaveProvider(provider.NAudioWaveFormat)
                {
                    DiscardOnBufferOverflow = true
                };

                provider.DataAvailable += (S, E) =>
                {
                    bufferedProvider.AddSamples(E.Buffer, 0, E.Length);
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
                var mixingSampleProvider = new MixingSampleProvider(_audioProviders.Values);

                // Screna expects 44.1 kHz 16-bit Stereo
                _mixingWaveProvider = mixingSampleProvider.ToWaveProvider16();
            }

            var bufferSize = (int)
            (
                (ReadInterval / 1000.0)
                * WaveFormat.SampleRate
                * WaveFormat.Channels
                * (WaveFormat.BitsPerSample / 8.0)
            );

            _buffer = new byte[bufferSize];

            Task.Factory.StartNew(Loop, TaskCreationOptions.LongRunning);
        }

        public void Dispose()
        {
            _continueEvent.Set();
            _stopEvent.Set();

            foreach (var provider in _audioProviders.Keys)
            {
                provider.Dispose();
            }

            _buffer = null;
        }

        public WaveFormat WaveFormat { get; } = new WaveFormat();

        public void Start()
        {
            foreach (var provider in _audioProviders.Keys)
            {
                provider.Start();
            }

            _continueEvent.Set();
        }

        public void Stop()
        {
            _continueEvent.Reset();

            foreach (var provider in _audioProviders.Keys)
            {
                provider.Stop();
            }
        }

        void Loop()
        {
            bool CanContinue()
            {
                try
                {
                    return _continueEvent.WaitOne() && !_stopEvent.WaitOne(0);
                }
                catch (ObjectDisposedException)
                {
                    return false;
                }
            }

            while (CanContinue())
            {
                _mixingWaveProvider.Read(_buffer, 0, _buffer.Length);

                DataAvailable?.Invoke(this, new DataAvailableEventArgs(_buffer, _buffer.Length));
             
                Thread.Sleep(ReadInterval);
            }
        }

        public event EventHandler<DataAvailableEventArgs> DataAvailable;
    }
}