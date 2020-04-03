using Captura.Audio;
using SharpDX.MediaFoundation;
using System;
using System.Runtime.InteropServices;

namespace Captura.Windows.MediaFoundation
{
    class MfAudioWriter : IAudioFileWriter
    {
        SinkWriter _writer;
        const int StreamIndex = 0;
        const int TenPower7 = 10_000_000;
        readonly long _audioInBytesPerSecond;
        long _audioWritten;

        public MfAudioWriter(string FileName,
            Guid MediaSubtype,
            WaveFormat Wf,
            int AudioQuality)
        {
            _writer = MediaFactory.CreateSinkWriterFromURL(FileName, null, null);

            _audioInBytesPerSecond = Wf.SampleRate * Wf.Channels * Wf.BitsPerSample / 8;

            using (var audioTypeOut = MfWriter.GetMediaType(Wf))
            {
                audioTypeOut.Set(MediaTypeAttributeKeys.Subtype, MediaSubtype);
                
                if (MediaSubtype == AudioFormatGuids.Aac)
                {
                    audioTypeOut.Set(MediaTypeAttributeKeys.AudioAvgBytesPerSecond, MfWriter.GetAacBitrate(AudioQuality));
                }

                _writer.AddStream(audioTypeOut, out _);
            }

            using (var audioTypeIn = MfWriter.GetMediaType(Wf))
            {
                audioTypeIn.Set(MediaTypeAttributeKeys.Subtype, AudioFormatGuids.Pcm);
                _writer.SetInputMediaType(StreamIndex, audioTypeIn, null);
            }

            _writer.BeginWriting();
        }

        public void Dispose()
        {
            _writer.Finalize();
            _writer.Dispose();
        }

        public void Flush()
        {
            _writer.Flush(StreamIndex);
        }

        public void Write(byte[] Data, int Offset, int Count)
        {
            using (var buffer = MediaFactory.CreateMemoryBuffer(Count))
            {
                var data = buffer.Lock(out _, out _);

                Marshal.Copy(Data, Offset, data, Count);

                buffer.CurrentLength = Count;

                buffer.Unlock();

                using var sample = MediaFactory.CreateVideoSampleFromSurface(null);
                sample.AddBuffer(buffer);

                sample.SampleTime = _audioWritten * TenPower7 / _audioInBytesPerSecond;
                sample.SampleDuration = Count * TenPower7 / _audioInBytesPerSecond;

                _writer.WriteSample(StreamIndex, sample);
            }

            _audioWritten += Count;
        }
    }
}
