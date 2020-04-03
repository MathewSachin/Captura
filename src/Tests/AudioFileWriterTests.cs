using System;
using System.IO;
using Captura.Audio;
using Xunit;

namespace Captura.Tests
{
    [Collection(nameof(Tests))]
    public class AudioFileWriterTests
    {
        [Fact]
        public void NullAudioOutputStream()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                using (new AudioFileWriter(OutStream: null, Format: new WaveFormat())) { }
            });
        }

        [Fact]
        public void NullWaveFormat()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                using (new AudioFileWriter(Stream.Null, null)) { }
            });
        }

        [Fact]
        public void NullFileName()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                using (new AudioFileWriter(FileName: null, Format: new WaveFormat())) { }
            });
        }
    }
}