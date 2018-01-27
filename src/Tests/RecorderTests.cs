using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Screna;
using Screna.Audio;

namespace Captura.Tests
{
    [TestClass]
    public class RecorderTests
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void NullVideoWriter()
        {
            var imageProvider = ServiceProvider.Get<IImageProvider>();

            using (new Recorder(null, imageProvider, 10))
            {
                
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void NullImageProvider()
        {
            var videoWriter = ServiceProvider.Get<IVideoFileWriter>();

            using (new Recorder(videoWriter, null, 10))
            {

            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void NullAudioWriter()
        {
            var audioProvider = ServiceProvider.Get<IAudioProvider>();

            using (new Recorder(null, audioProvider))
            {

            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void NullAudioProvider()
        {
            var audioWriter = ServiceProvider.Get<IAudioFileWriter>();

            using (new Recorder(audioWriter, null))
            {

            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void NullGifWriter()
        {
            var imageProvider = ServiceProvider.Get<IImageProvider>();

            using (new VFRGifRecorder(null, imageProvider))
            {

            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void NegativeFrameRate()
        {
            var imageProvider = ServiceProvider.Get<IImageProvider>();
            var videoWriter = ServiceProvider.Get<IVideoFileWriter>();

            using (new Recorder(videoWriter, imageProvider, -1))
            {
                
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ZeroFrameRate()
        {
            var imageProvider = ServiceProvider.Get<IImageProvider>();
            var videoWriter = ServiceProvider.Get<IVideoFileWriter>();

            using (new Recorder(videoWriter, imageProvider, 0))
            {

            }
        }
    }
}
