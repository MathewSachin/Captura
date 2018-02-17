using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
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
            var imageProvider = MoqFactory.GetImageProviderMock().Object;

            using (new Recorder(null, imageProvider, 10))
            {
                
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void NullImageProvider()
        {
            var videoWriter = MoqFactory.GetVideoFileWriterMock().Object;

            using (new Recorder(videoWriter, null, 10))
            {

            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void NullAudioWriter()
        {
            var audioProvider = MoqFactory.GetAudioProviderMock().Object;

            using (new Recorder(null, audioProvider))
            {

            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void NullAudioProvider()
        {
            var audioWriter = MoqFactory.GetAudioFileWriterMock().Object;

            using (new Recorder(audioWriter, null))
            {

            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void NullGifWriter()
        {
            var imageProvider = MoqFactory.GetImageProviderMock().Object;

            using (new VFRGifRecorder(null, imageProvider))
            {

            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void NegativeFrameRate()
        {
            var imageProvider = MoqFactory.GetImageProviderMock().Object;
            var videoWriter = MoqFactory.GetVideoFileWriterMock().Object;

            using (new Recorder(videoWriter, imageProvider, -1))
            {
                
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ZeroFrameRate()
        {
            var imageProvider = MoqFactory.GetImageProviderMock().Object;
            var videoWriter = MoqFactory.GetVideoFileWriterMock().Object;

            using (new Recorder(videoWriter, imageProvider, 0))
            {

            }
        }

        [TestMethod]
        public void RecorderVideoDispose()
        {
            var imgProviderMock = MoqFactory.GetImageProviderMock();
            var videoWriterMock = MoqFactory.GetVideoFileWriterMock();
            var audioProviderMock = MoqFactory.GetAudioProviderMock();

            using (new Recorder(videoWriterMock.Object, imgProviderMock.Object, 10, audioProviderMock.Object))
            {
                
            }

            imgProviderMock.Verify(M => M.Dispose(), Times.Once);
            videoWriterMock.Verify(M => M.Dispose(), Times.Once);
            audioProviderMock.Verify(M => M.Dispose(), Times.Once);
        }

        [TestMethod]
        public void RecorderAudioDispose()
        {
            var audioWriterMock = MoqFactory.GetAudioFileWriterMock();
            var audioProviderMock = MoqFactory.GetAudioProviderMock();

            using (new Recorder(audioWriterMock.Object, audioProviderMock.Object))
            {

            }
            
            audioWriterMock.Verify(M => M.Dispose(), Times.Once);
            audioProviderMock.Verify(M => M.Dispose(), Times.Once);
        }

        [TestMethod]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void StartAfterDisposed()
        {
            var imageProvider = MoqFactory.GetImageProviderMock().Object;
            var videoWriter = MoqFactory.GetVideoFileWriterMock().Object;

            var recorder = new Recorder(videoWriter, imageProvider, 10);

            using (recorder)
            {
            }

            recorder.Start();
        }

        [TestMethod]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void StopAfterDisposed()
        {
            var imageProvider = MoqFactory.GetImageProviderMock().Object;
            var videoWriter = MoqFactory.GetVideoFileWriterMock().Object;

            var recorder = new Recorder(videoWriter, imageProvider, 10);

            using (recorder)
            {
            }

            recorder.Stop();
        }

        [TestMethod]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void DisposeTwice()
        {
            var imageProvider = MoqFactory.GetImageProviderMock().Object;
            var videoWriter = MoqFactory.GetVideoFileWriterMock().Object;

            var recorder = new Recorder(videoWriter, imageProvider, 10);

            using (recorder)
            {
            }

            using (recorder)
            {
            }
        }
    }
}
