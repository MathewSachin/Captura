using System.IO;
using SharpAvi.Codecs;
using ScrenaAudioEncoder = Screna.Audio.IAudioEncoder;

namespace Screna.Avi
{
    class IAudioEncoderWrapper : IAudioEncoder
    {
        readonly ScrenaAudioEncoder _encoder;

        public IAudioEncoderWrapper(ScrenaAudioEncoder Encoder)
        {
            _encoder = Encoder;
        }

        public int BitsPerSample => _encoder.WaveFormat.BitsPerSample;

        public int BytesPerSecond => _encoder.WaveFormat.AverageBytesPerSecond;

        public int ChannelCount => _encoder.WaveFormat.Channels;

        public short Format => (short)_encoder.WaveFormat.Encoding;

        public byte[] FormatSpecificData
        {
            get
            {
                var extraSize = _encoder.WaveFormat.ExtraSize;

                if (extraSize <= 0)
                    return null;

                using (var ms = new MemoryStream())
                    using (var writer = new BinaryWriter(ms))
                {
                    _encoder.WaveFormat.Serialize(writer);

                    var formatData = new byte[extraSize];

                    ms.Seek(18, SeekOrigin.Begin);

                    ms.Read(formatData, 0, extraSize);

                    return formatData;
                }
            }
        }

        public int Granularity => _encoder.WaveFormat.BlockAlign;

        public int SamplesPerSecond => _encoder.WaveFormat.SampleRate;

        public int EncodeBlock(byte[] source, int sourceOffset, int sourceCount, byte[] destination, int destinationOffset)
        {
            return _encoder.Encode(source, sourceOffset, sourceCount, destination, destinationOffset);
        }

        public int Flush(byte[] destination, int destinationOffset) => _encoder.Flush(destination, destinationOffset);

        public int GetMaxEncodedLength(int sourceCount) => _encoder.GetMaxEncodedLength(sourceCount);
    }
}