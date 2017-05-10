using System;
using System.IO;
using Screna.Audio;
using IAudioEncoder = SharpAvi.Codecs.IAudioEncoder;

namespace Captura.Models
{
    /// <summary>
    /// Enables a Screna based Audio Provider to be used with SharpAvi.
    /// </summary>
    class IAudioProviderAdapter : IAudioEncoder
    {
        readonly IAudioProvider _provider;

        public IAudioProviderAdapter(IAudioProvider Provider)
        {
            _provider = Provider;
        }

        public int BitsPerSample => _provider.WaveFormat.BitsPerSample;

        public int BytesPerSecond => _provider.WaveFormat.AverageBytesPerSecond;

        public int ChannelCount => _provider.WaveFormat.Channels;

        public short Format => (short)_provider.WaveFormat.Encoding;

        public byte[] FormatSpecificData
        {
            get
            {
                var extraSize = _provider.WaveFormat.ExtraSize;

                if (extraSize <= 0)
                    return null;

                using (var ms = new MemoryStream())
                    using (var writer = new BinaryWriter(ms))
                    {
                        _provider.WaveFormat.Serialize(writer);

                        var formatData = new byte[extraSize];

                        ms.Seek(18, SeekOrigin.Begin);

                        ms.Read(formatData, 0, extraSize);

                        return formatData;
                    }
            }
        }

        public int Granularity => _provider.WaveFormat.BlockAlign;

        public int SamplesPerSecond => _provider.WaveFormat.SampleRate;

        public int EncodeBlock(byte[] source, int sourceOffset, int sourceCount, byte[] destination, int destinationOffset)
        {
            Array.Copy(source, sourceOffset, destination, destinationOffset, sourceCount);

            return sourceCount;
        }

        public int Flush(byte[] destination, int destinationOffset) => 0;

        public int GetMaxEncodedLength(int sourceCount) => sourceCount;
    }
}