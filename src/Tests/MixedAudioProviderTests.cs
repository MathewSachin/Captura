using System;
using System.IO;
using Captura.Audio;
using NAudio.Wave;
using Xunit;
using Wf = NAudio.Wave.WaveFormat;

namespace Captura.Tests
{
    [Collection(nameof(Tests))]
    public class MixedAudioProviderTests
    {
        class FakeWaveIn : IWaveIn
        {
            public Wf WaveFormat { get; set; }

            public void StartRecording() { }

            public void StopRecording() { }

            public void Dispose() { }

            public event EventHandler<WaveInEventArgs> DataAvailable;

            public event EventHandler<StoppedEventArgs> RecordingStopped { add { } remove { } }

            public void Push(byte[] Data)
            {
                DataAvailable?.Invoke(this, new WaveInEventArgs(Data, Data.Length));
            }
        }

        class FakeNAudioProvider : NAudioProvider
        {
            public FakeNAudioProvider(IWaveIn WaveIn) : base(WaveIn) { }
        }

        [Theory]
        [InlineData(16000, 1, false)]
        [InlineData(48000, 2, true)]
        [InlineData(44100, 2, true)]
        public void BurstySourceIsResampledWithoutDipsOrLoss(int SampleRate, int Channels, bool IsFloat)
        {
            var waveIn = new FakeWaveIn
            {
                WaveFormat = IsFloat
                    ? Wf.CreateIeeeFloatWaveFormat(SampleRate, Channels)
                    : new Wf(SampleRate, 16, Channels)
            };

            var provider = new MixedAudioProvider(new FakeNAudioProvider(waveIn));

            var burstsMs = new[] { 5, 0, 7, 0, 0, 3 };
            var bytesPerSample = IsFloat ? 4 : 2;
            var totalFrames = SampleRate * 3;
            var pushedFrames = 0;

            // 100 ms of 44.1 kHz 16-bit stereo
            var readBuffer = new byte[17640];
            var output = new MemoryStream();

            for (var burst = 0; pushedFrames < totalFrames; ++burst)
            {
                var frames = Math.Min(SampleRate * burstsMs[burst % burstsMs.Length] / 1000, totalFrames - pushedFrames);
                var data = new byte[frames * Channels * bytesPerSample];

                for (var i = 0; i < frames; ++i)
                {
                    // Between 1000 and 4000, never near zero
                    var value = 1000 + 3000 * (1 + Math.Sin(2 * Math.PI * 300 * (pushedFrames + i) / SampleRate)) / 2;

                    var sample = IsFloat
                        ? BitConverter.GetBytes((float)(value / 32768))
                        : BitConverter.GetBytes((short)value);

                    for (var channel = 0; channel < Channels; ++channel)
                    {
                        sample.CopyTo(data, (i * Channels + channel) * bytesPerSample);
                    }
                }

                waveIn.Push(data);
                pushedFrames += frames;

                output.Write(readBuffer, 0, provider.Read(readBuffer, 0, readBuffer.Length));
            }

            for (var i = 0; i < 200; ++i)
            {
                var read = provider.Read(readBuffer, 0, readBuffer.Length);

                if (read == 0)
                    break;

                output.Write(readBuffer, 0, read);
            }

            var bytes = output.ToArray();
            var minSample = int.MaxValue;

            // Skip the resampler's filter warm-up
            for (var i = 4410 * 2; i + 1 < bytes.Length; i += 2)
            {
                minSample = Math.Min(minSample, BitConverter.ToInt16(bytes, i));
            }

            double outputFrames = bytes.Length / 4;
            var expected = pushedFrames * 44100.0 / SampleRate;

            Assert.True(minSample >= 500, $"Min sample: {minSample}, output frames: {outputFrames}, expected: {expected}");
            Assert.InRange(outputFrames, expected * 0.995, expected * 1.005);
        }
    }
}
