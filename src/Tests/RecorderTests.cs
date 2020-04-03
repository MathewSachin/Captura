using System;
using Captura.Audio;
using Captura.Video;
using Moq;
using Xunit;

namespace Captura.Tests
{
    [Collection(nameof(Tests))]
    public class RecorderTests
    {
        readonly MoqFixture _moq;

        public RecorderTests(MoqFixture Moq)
        {
            _moq = Moq;
        }

        [Fact]
        public void NullVideoWriter()
        {
            var imageProvider = _moq.GetImageProviderMock().Object;

            Assert.Throws<ArgumentNullException>(() =>
            {
                using (new Recorder(null, imageProvider, 10)) { }
            });
        }

        [Fact]
        public void NullImageProvider()
        {
            var videoWriter = _moq.GetVideoFileWriterMock().Object;

            Assert.Throws<ArgumentNullException>(() =>
            {
                using (new Recorder(videoWriter, null, 10)) { }
            });
        }

        [Fact]
        public void NullAudioWriter()
        {
            var audioProvider = _moq.GetAudioProviderMock().Object;

            Assert.Throws<ArgumentNullException>(() =>
            {
                using (new AudioRecorder(null, audioProvider)) { }
            });
        }

        [Fact]
        public void NullAudioProvider()
        {
            var audioWriter = _moq.GetAudioFileWriterMock().Object;

            Assert.Throws<ArgumentNullException>(() =>
            {
                using (new AudioRecorder(audioWriter, null)) { }
            });
        }

        [Fact]
        public void NegativeFrameRate()
        {
            var imageProvider = _moq.GetImageProviderMock().Object;
            var videoWriter = _moq.GetVideoFileWriterMock().Object;

            Assert.Throws<ArgumentException>(() =>
            {
                using (new Recorder(videoWriter, imageProvider, -1)) { }
            });
        }

        [Fact]
        public void ZeroFrameRate()
        {
            var imageProvider = _moq.GetImageProviderMock().Object;
            var videoWriter = _moq.GetVideoFileWriterMock().Object;

            Assert.Throws<ArgumentException>(() =>
            {
                using (new Recorder(videoWriter, imageProvider, 0)) { }
            });
        }

        [Fact]
        public void RecorderVideoDispose()
        {
            var imgProviderMock = _moq.GetImageProviderMock();
            var videoWriterMock = _moq.GetVideoFileWriterMock();
            var audioProviderMock = _moq.GetAudioProviderMock();

            using (new Recorder(videoWriterMock.Object, imgProviderMock.Object, 10, audioProviderMock.Object)) 
            {
            }

            imgProviderMock.Verify(M => M.Dispose(), Times.Once);
            videoWriterMock.Verify(M => M.Dispose(), Times.Once);
            audioProviderMock.Verify(M => M.Dispose(), Times.Once);
        }

        [Fact]
        public void RecorderAudioDispose()
        {
            var audioWriterMock = _moq.GetAudioFileWriterMock();
            var audioProviderMock = _moq.GetAudioProviderMock();

            using (new AudioRecorder(audioWriterMock.Object, audioProviderMock.Object))
            {
            }
            
            audioWriterMock.Verify(M => M.Dispose(), Times.Once);
            audioProviderMock.Verify(M => M.Dispose(), Times.Once);
        }

        [Fact]
        public void StartAfterDisposed()
        {
            var imageProvider = _moq.GetImageProviderMock().Object;
            var videoWriter = _moq.GetVideoFileWriterMock().Object;

            var recorder = new Recorder(videoWriter, imageProvider, 10);

            using (recorder)
            {
            }

            Assert.Throws<ObjectDisposedException>(() => recorder.Start());
        }

        [Fact]
        public void StopAfterDisposed()
        {
            var imageProvider = _moq.GetImageProviderMock().Object;
            var videoWriter = _moq.GetVideoFileWriterMock().Object;

            var recorder = new Recorder(videoWriter, imageProvider, 10);

            using (recorder)
            {
            }

            Assert.Throws<ObjectDisposedException>(() => recorder.Stop());
        }
    }
}
