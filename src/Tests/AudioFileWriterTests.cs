using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Screna.Audio;

namespace Captura.Tests
{
    [TestClass]
    public class AudioFileWriterTests
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void NullAudioOutputStream()
        {
            using (new AudioFileWriter(OutStream: null, Format: new WaveFormat()))
            {
                
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void NullWaveFormat()
        {
            using (new AudioFileWriter(Stream.Null, null))
            {

            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void NullFileName()
        {
            using (new AudioFileWriter(FileName: null, Format: new WaveFormat()))
            {

            }
        }
    }
}