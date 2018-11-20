using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Captura.Audio;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using WaveFormat = Captura.Audio.WaveFormat;
using Wf = NAudio.Wave.WaveFormat;

namespace Captura.NAudio
{
    class MixedAudioProvider : IAudioProvider
    {
        readonly Dictionary<NAudioProvider, ISampleProvider> _audioProviders = new Dictionary<NAudioProvider, ISampleProvider>();

        readonly IWaveProvider _mixingWaveProvider;

        readonly ManualResetEvent _continueEvent = new ManualResetEvent(false),
            _stopEvent = new ManualResetEvent(false);

        readonly byte[] _buffer;
        const int ReadInterval = 200;

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

            var mixingSampleProvider = new MixingSampleProvider(_audioProviders.Values);

            _mixingWaveProvider = mixingSampleProvider.ToWaveProvider16();

            _buffer = new byte[(int)(ReadInterval / 1000.0 * 44100 * 2 * 2)]; // s * freq * chans * 16-bit

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